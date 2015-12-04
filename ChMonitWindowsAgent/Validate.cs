using ChMonitoring.Helpers;
using ChMonitoring.MonitData;
using System;

namespace ChMonitoring
{
    /// <summary>
    /// TODO
    /// </summary>
    class Validate
    {
        #region Public Methods

        public static int validate()
        {
            int errors = 0;

            MonitWindowsAgent.Run.handler_flag = MonitHandlerType.Handler_Succeeded;
            Event.EventQueueProcess();

            //updateSystemLoad();
            //initprocesstree(&ptree, &ptreesize, &oldptree, &oldptreesize);
            MonitWindowsAgent.systeminfo.collected = DateTime.UtcNow;

            /* In the case that at least one action is pending, perform quick loop to handle the actions ASAP */
            if (MonitWindowsAgent.Run.doaction)
            {
                MonitWindowsAgent.Run.doaction = false;
                foreach (var s in MonitWindowsAgent.servicelist)
                    doScheduledAction(s);
            }

            /* Check the services */
            foreach (var s in MonitWindowsAgent.servicelist)
            {
                if (MonitWindowsAgent.Run.stopped)
                    break;
                if (!doScheduledAction(s) && s.monitor != MonitMonitorStateType.Monitor_Not && !checkSkip(s))
                {
                    checkTimeout(s); // Can disable monitoring => need to check s.monitor again
                    if (s.monitor != MonitMonitorStateType.Monitor_Not)
                    {
                        if (s.check == null || !s.check(s))
                            errors++;
                        /* The monitoring may be disabled by some matching rule in s.check
                         * so we have to check again before setting to Monitor_Yes */
                        if (s.monitor != MonitMonitorStateType.Monitor_Not)
                            s.monitor = MonitMonitorStateType.Monitor_Yes;
                    }
                    s.collected = DateTime.UtcNow;
                }
            }

            Control.ResetDepend();

            return errors;
        }

        /**
         * Validate a given process service s. Events are posted according to
         * its configuration. In case of a fatal event false is returned.
         */
        public static bool CheckProcess(Service_T s)
        {
            int pid = Util.IsProcessRunning(s, false);
            if (pid == 0)
            {
                foreach (var l in s.nonexistlist)
                    Event.Post(s, MonitEventType.Event_Nonexist, MonitStateType.State_Failed, l, "process is not running");
                return false;
            }
            else
            {
                foreach (var l in s.nonexistlist)
                    Event.Post(s, MonitEventType.Event_Nonexist, MonitStateType.State_Succeeded, l, "process is running with pid %d", pid);
            }
            /* Reset the exec and timeout errors if active ... the process is running (most probably after manual intervention) */
            if (Event.IsEventSet(s.error, MonitEventType.Event_Exec))
                Event.Post(s, MonitEventType.Event_Exec, MonitStateType.State_Succeeded, s.action_EXEC, "process is running after previous exec error (slow starting or manually recovered?)");
            if (Event.IsEventSet(s.error, MonitEventType.Event_Timeout))
                foreach (var ar in s.actionratelist)
                    Event.Post(s, MonitEventType.Event_Timeout, MonitStateType.State_Succeeded, ar.action, "process is running after previous restart timeout (manually recovered?)");
            if (MonitWindowsAgent.Run.doprocess)
            {
                //if (update_process_data(s, ptree, ptreesize, pid))
                //{
                    checkProcessState(s);
                    checkProcessPid(s);
                    checkProcessPpid(s);
                    if (s.uid != null)
                        checkUid(s, (s.inf as ProcessInfo_T).uid);
                    if (s.euid != null)
                        checkEuid(s, (s.inf as ProcessInfo_T).euid);
                    if (s.gid != null)
                        checkGid(s, (s.inf as ProcessInfo_T).gid);
                if (s.uptimelist != null)
                    checkUptime(s);
                foreach (var pr in s.resourcelist)
                    checkProcessResources(s, pr);
                //}
                //else
                //{
                Logger.Log.ErrorFormat("'{0}' failed to get service data", s.name);
                //}
            }
            //if (s.portlist)
            //{
            //    /* pause port tests in the start timeout timeframe while the process is starting (it may take some time to the process before it starts accepting connections) */
            //    if (!s.start || s.inf.priv.process.uptime > s.start.timeout)
            //        for (Port_T pp = s.portlist; pp; pp = pp.next)
            //            check_connection(s, pp);
            //}
            //if (s.socketlist)
            //{
            //    /* pause socket tests in the start timeout timeframe while the process is starting (it may take some time to the process before it starts accepting connections) */
            //    if (!s.start || s.inf.priv.process.uptime > s.start.timeout)
            //        for (Port_T pp = s.socketlist; pp; pp = pp.next)
            //            check_connection(s, pp);
            //}
            return true;
        }

        /**
         * Validate a given filesystem service s. Events are posted according to
         * its configuration. In case of a fatal event false is returned.
         */
        public static bool CheckFilesystem(Service_T s)
        {
            //if (!filesystem_usage(s))
            //{
            //    Event.EventPost(s, MonitEventType.Event_Data, MonitStateType.State_Failed, s.action_DATA, "unable to read filesystem '{0}' state", s.path);
            //    return false;
            //}
            Event.Post(s, MonitEventType.Event_Data, MonitStateType.State_Succeeded, s.action_DATA, "succeeded getting filesystem statistics for '{0}'", s.path);

            //if (s.perm)
            //    check_perm(s, s.inf.priv.filesystem.mode);

            if (s.uid != null)
                checkUid(s, (s.inf as FileSystemInfo_T).uid);

            if (s.gid != null)
                checkGid(s, (s.inf as FileSystemInfo_T).gid);

            //check_filesystem_flags(s);

            foreach (var td in s.filesystemlist)
                checkFilesystemResources(s, td);

            return true;
        }

        //        /**
        // * Validate a given file service s. Events are posted according to
        // * its configuration. In case of a fatal event false is returned.
        // */
        //boolean_t check_file(Service_T s) {
        //        struct stat stat_buf;

        //        ASSERT(s);

        //        if (stat(s.path, &stat_buf) != 0) {
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Failed, l.action, "file doesn't exist");
        //                return false;
        //        } else {
        //                s.inf.priv.file.mode = stat_buf.st_mode;
        //                if (s.inf.priv.file.inode)
        //                        s.inf.priv.file.inode_prev = s.inf.priv.file.inode;
        //                s.inf.priv.file.inode = stat_buf.st_ino;
        //                s.inf.priv.file.uid = stat_buf.st_uid;
        //                s.inf.priv.file.gid = stat_buf.st_gid;
        //                s.inf.priv.file.size = stat_buf.st_size;
        //                s.inf.priv.file.timestamp = MAX(stat_buf.st_mtime, stat_buf.st_ctime);
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Succeeded, l.action, "file exists");
        //        }

        //        if (! S_ISREG(s.inf.priv.file.mode) && ! S_ISSOCK(s.inf.priv.file.mode)) {
        //                Event_post(s, Event_Invalid, State_Failed, s.action_INVALID, "is neither a regular file nor a socket");
        //                return false;
        //        } else {
        //                Event_post(s, Event_Invalid, State_Succeeded, s.action_INVALID, "is a regular file or socket");
        //        }

        //        if (s.checksum)
        //                check_checksum(s);

        //        if (s.perm)
        //                check_perm(s, s.inf.priv.file.mode);

                //if (s.uid)
                //        check_uid(s, s.inf.priv.file.uid);

        //        if (s.gid)
        //                check_gid(s, s.inf.priv.file.gid);

        //        if (s.sizelist)
        //                check_size(s);

        //        if (s.timestamplist)
        //                check_timestamp(s, s.inf.priv.file.timestamp);

        //        if (s.matchlist)
        //                check_match(s);

        //        return true;

        //}


        ///**
        // * Validate a given directory service s. Events are posted according to
        // * its configuration. In case of a fatal event false is returned.
        // */
        //boolean_t check_directory(Service_T s) {

        //        struct stat stat_buf;

        //        ASSERT(s);

        //        if (stat(s.path, &stat_buf) != 0) {
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Failed, l.action, "directory doesn't exist");
        //                return false;
        //        } else {
        //                s.inf.priv.directory.mode = stat_buf.st_mode;
        //                s.inf.priv.directory.uid = stat_buf.st_uid;
        //                s.inf.priv.directory.gid = stat_buf.st_gid;
        //                s.inf.priv.directory.timestamp = MAX(stat_buf.st_mtime, stat_buf.st_ctime);
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Succeeded, l.action, "directory exists");
        //        }

        //        if (! S_ISDIR(s.inf.priv.directory.mode)) {
        //                Event_post(s, Event_Invalid, State_Failed, s.action_INVALID, "is not directory");
        //                return false;
        //        } else {
        //                Event_post(s, Event_Invalid, State_Succeeded, s.action_INVALID, "is directory");
        //        }

        //        if (s.perm)
        //                check_perm(s, s.inf.priv.directory.mode);

        //        if (s.uid)
        //                check_uid(s, s.inf.priv.directory.uid);

        //        if (s.gid)
        //                check_gid(s, s.inf.priv.directory.gid);

        //        if (s.timestamplist)
        //                check_timestamp(s, s.inf.priv.directory.timestamp);

        //        return true;

        //}


        ///**
        // * Validate a given fifo service s. Events are posted according to
        // * its configuration. In case of a fatal event false is returned.
        // */
        //boolean_t check_fifo(Service_T s) {

        //        struct stat stat_buf;

        //        ASSERT(s);

        //        if (stat(s.path, &stat_buf) != 0) {
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Failed, l.action, "fifo doesn't exist");
        //                return false;
        //        } else {
        //                s.inf.priv.fifo.mode = stat_buf.st_mode;
        //                s.inf.priv.fifo.uid = stat_buf.st_uid;
        //                s.inf.priv.fifo.gid = stat_buf.st_gid;
        //                s.inf.priv.fifo.timestamp = MAX(stat_buf.st_mtime, stat_buf.st_ctime);
        //                for (Nonexist_T l = s.nonexistlist; l; l = l.next)
        //                        Event_post(s, Event_Nonexist, State_Succeeded, l.action, "fifo exists");
        //        }

        //        if (! S_ISFIFO(s.inf.priv.fifo.mode)) {
        //                Event_post(s, Event_Invalid, State_Failed, s.action_INVALID, "is not fifo");
        //                return false;
        //        } else {
        //                Event_post(s, Event_Invalid, State_Succeeded, s.action_INVALID, "is fifo");
        //        }

        //        if (s.perm)
        //                check_perm(s, s.inf.priv.fifo.mode);

        //        if (s.uid)
        //                check_uid(s, s.inf.priv.fifo.uid);

        //        if (s.gid)
        //                check_gid(s, s.inf.priv.fifo.gid);

        //        if (s.timestamplist)
        //                check_timestamp(s, s.inf.priv.fifo.timestamp);

        //        return true;

        //}


        ///**
        // * Validate a program status. Events are posted according to
        // * its configuration. In case of a fatal event false is returned.
        // */
        //boolean_t check_program(Service_T s) {
        //        ASSERT(s);
        //        ASSERT(s.program);
        //        time_t now = Time_now();
        //        Process_T P = s.program.P;
        //        if (P) {
        //                if (Process_exitStatus(P) < 0) { // Program is still running
        //                        time_t execution_time = (now - s.program.started);
        //                        if (execution_time > s.program.timeout) { // Program timed out
        //                                LogError("'%s' program timed out after %lld seconds. Killing program with pid %ld\n", s.name, (long long)execution_time, (long)Process_getPid(P));
        //                                Process_kill(P);
        //                                Process_waitFor(P); // Wait for child to exit to get correct exit value
        //                                // Fall-through with P and evaluate exit value below.
        //                        } else {
        //                                // Defer test of exit value until program exit or timeout
        //                                DEBUG("'%s' status check defered - waiting on program to exit\n", s.name);
        //                                return true;
        //                        }
        //                }
        //                s.program.exitStatus = Process_exitStatus(P); // Save exit status for web-view display
        //                // Save program output
        //                StringBuffer_clear(s.program.output);
        //                _programOutput(Process_getErrorStream(P), s.program.output);
        //                _programOutput(Process_getInputStream(P), s.program.output);
        //                StringBuffer_trim(s.program.output);
        //                // Evaluate program's exit status against our status checks.
        //                /* TODO: Multiple checks we have now should be deprecated and removed - not useful because it
        //                 will alert on everything if != is used other than the match or if = is used, might report nothing on error. */
        //                for (Status_T status = s.statuslist; status; status = status.next) {
        //                        if (status.operator == Operator_Changed) {
        //                                if (status.initialized) {
        //                                        if (Util_evalQExpression(status.operator, s.program.exitStatus, status.return_value)) {
        //                                                Event_post(s, Event_Status, State_Changed, status.action, "program status changed (%d . %d) -- %s", status.return_value, s.program.exitStatus, StringBuffer_length(s.program.output) ? StringBuffer_toString(s.program.output) : "no output");
        //                                                status.return_value = s.program.exitStatus;
        //                                        } else {
        //                                                Event_post(s, Event_Status, State_ChangedNot, status.action, "program status didn't change [status=%d] -- %s", s.program.exitStatus, StringBuffer_length(s.program.output) ? StringBuffer_toString(s.program.output) : "no output");
        //                                        }
        //                                } else {
        //                                        status.initialized = true;
        //                                        status.return_value = s.program.exitStatus;
        //                                }
        //                        } else {
        //                                if (Util_evalQExpression(status.operator, s.program.exitStatus, status.return_value))
        //                                        Event_post(s, Event_Status, State_Failed, status.action, "'%s' failed with exit status (%d) -- %s", s.path, s.program.exitStatus, StringBuffer_length(s.program.output) ? StringBuffer_toString(s.program.output) : "no output");
        //                                else
        //                                        Event_post(s, Event_Status, State_Succeeded, status.action, "status succeeded [status=%d] -- %s", s.program.exitStatus, StringBuffer_length(s.program.output) ? StringBuffer_toString(s.program.output) : "no output");
        //                        }
        //                }
        //                Process_free(&s.program.P);
        //        }
        //        // Start program
        //        s.program.P = Command_execute(s.program.C);
        //        if (! s.program.P) {
        //                Event_post(s, Event_Status, State_Failed, s.action_EXEC, "failed to execute '%s' -- %s", s.path, STRERROR);
        //        } else {
        //                Event_post(s, Event_Status, State_Succeeded, s.action_EXEC, "'%s' program started", s.name);
        //                s.program.started = now;
        //        }
        //        return true;
        //}


        ///**
        // * Validate a remote service.
        // * @param s The remote service to validate
        // * @return false if there was an error otherwise true
        // */
        //boolean_t check_remote_host(Service_T s) {
        //        ASSERT(s);

        //        Icmp_T last_ping = NULL;

        //        /* Test each icmp type in the service's icmplist */
        //        for (Icmp_T icmp = s.icmplist; icmp; icmp = icmp.next) {

        //                switch (icmp.type) {
        //                        case ICMP_ECHO:

        //                                icmp.response = icmp_echo(s.path, icmp.family, icmp.timeout, icmp.count);

        //                                if (icmp.response == -2) {
        //                                        icmp.is_available = true;
        //#ifdef SOLARIS
        //                                        DEBUG("'%s' ping test skipped -- the monit user has no permission to create raw socket, please add net_icmpaccess privilege\n", s.name);
        //#else
        //                                        DEBUG("'%s' ping test skipped -- the monit user has no permission to create raw socket, please run monit as root\n", s.name);
        //#endif
        //                                } else if (icmp.response == -1) {
        //                                        icmp.is_available = false;
        //                                        Event_post(s, Event_Icmp, State_Failed, icmp.action, "ping test failed");
        //                                } else {
        //                                        icmp.is_available = true;
        //                                        Event_post(s, Event_Icmp, State_Succeeded, icmp.action, "ping test succeeded [response time %.3fs]", icmp.response);
        //                                }
        //                                last_ping = icmp;
        //                                break;

        //                        default:
        //                                LogError("'%s' error -- unknown ICMP type: [%d]\n", s.name, icmp.type);
        //                                return false;

        //                }
        //        }

        //        /* If we could not ping the host we assume it's down and do not
        //         * continue to check any port connections  */
        //        if (last_ping && ! last_ping.is_available) {
        //                DEBUG("'%s' icmp ping failed, skipping any port connection tests\n", s.name);
        //                return false;
        //        }

        //        /* Test each host:port and protocol in the service's portlist */
        //        for (Port_T p = s.portlist; p; p = p.next)
        //                check_connection(s, p);

        //        return true;

        //}


        /**
         * Validate the general system indicators. In case of a fatal event
         * false is returned.
         */
        public static bool CheckSystem(Service_T s)
        {
            foreach (var r in s.resourcelist)
                checkProcessResources(s, r);
            return true;
        }


        //boolean_t check_net(Service_T s) {
        //        boolean_t havedata = true;
        //        TRY
        //        {
        //                Link_update(s.inf.priv.net.stats);
        //        }
        //        ELSE
        //        {
        //                havedata = false;
        //                for (LinkStatus_T link = s.linkstatuslist; link; link = link.next)
        //                        Event_post(s, Event_Link, State_Failed, link.action, "link data gathering failed -- %s", Exception_frame.message);
        //        }
        //        END_TRY;
        //        if (! havedata)
        //                return false; // Terminate test if no data are available
        //        for (LinkStatus_T link = s.linkstatuslist; link; link = link.next) {
        //                Event_post(s, Event_Size, State_Succeeded, link.action, "link data gathering succeeded");
        //        }
        //        // State
        //        if (! Link_getState(s.inf.priv.net.stats)) {
        //                for (LinkStatus_T link = s.linkstatuslist; link; link = link.next)
        //                        Event_post(s, Event_Link, State_Failed, link.action, "link down");
        //                return false; // Terminate test if the link is down
        //        } else {
        //                for (LinkStatus_T link = s.linkstatuslist; link; link = link.next)
        //                        Event_post(s, Event_Link, State_Succeeded, link.action, "link up");
        //        }
        //        // Link errors
        //        long long oerrors = Link_getErrorsOutPerSecond(s.inf.priv.net.stats);
        //        for (LinkStatus_T link = s.linkstatuslist; link; link = link.next) {
        //                if (oerrors)
        //                        Event_post(s, Event_Link, State_Failed, link.action, "%lld upload errors detected", oerrors);
        //                else
        //                        Event_post(s, Event_Link, State_Succeeded, link.action, "upload errors check succeeded");
        //        }
        //        long long ierrors = Link_getErrorsInPerSecond(s.inf.priv.net.stats);
        //        for (LinkStatus_T link = s.linkstatuslist; link; link = link.next) {
        //                if (ierrors)
        //                        Event_post(s, Event_Link, State_Failed, link.action, "%lld download errors detected", ierrors);
        //                else
        //                        Event_post(s, Event_Link, State_Succeeded, link.action, "download errors check succeeded");
        //        }
        //        // Link speed
        //        int duplex = Link_getDuplex(s.inf.priv.net.stats);
        //        long long speed = Link_getSpeed(s.inf.priv.net.stats);
        //        for (LinkSpeed_T link = s.linkspeedlist; link; link = link.next) {
        //                if (speed > 0 && link.speed) {
        //                        if (duplex > -1 && duplex != link.duplex)
        //                                Event_post(s, Event_Speed, State_Changed, link.action, "link mode is now %s-duplex", duplex ? "full" : "half");
        //                        else
        //                                Event_post(s, Event_Speed, State_ChangedNot, link.action, "link mode has not changed since last cycle [current mode is %s-duplex]", duplex ? "full" : "half");
        //                        if (speed != link.speed)
        //                                Event_post(s, Event_Speed, State_Changed, link.action, "link speed changed to %.0lf Mb/s", (double)speed / 1000000.);
        //                        else
        //                                Event_post(s, Event_Speed, State_ChangedNot, link.action, "link speed has not changed since last cycle [current speed = %.0lf Mb/s]", (double)speed / 1000000.);
        //                }
        //                link.duplex = duplex;
        //                link.speed = speed;
        //        }
        //        // Link saturation
        //        double osaturation = Link_getSaturationOutPerSecond(s.inf.priv.net.stats);
        //        double isaturation = Link_getSaturationInPerSecond(s.inf.priv.net.stats);
        //        if (osaturation >= 0. && isaturation >= 0.) {
        //                for (LinkSaturation_T link = s.linksaturationlist; link; link = link.next) {
        //                        if (duplex) {
        //                                if (Util_evalDoubleQExpression(link.operator, osaturation, link.limit))
        //                                        Event_post(s, Event_Saturation, State_Failed, link.action, "link upload saturation of %.1f%% matches limit [saturation %s %.1f%%]", osaturation, operatorshortnames[link.operator], link.limit);
        //                                else
        //                                        Event_post(s, Event_Saturation, State_Succeeded, link.action, "link upload saturation check succeeded [current upload saturation %.1f%%]", osaturation);
        //                                if (Util_evalDoubleQExpression(link.operator, isaturation, link.limit))
        //                                        Event_post(s, Event_Saturation, State_Failed, link.action, "link download saturation of %.1f%% matches limit [saturation %s %.1f%%]", isaturation, operatorshortnames[link.operator], link.limit);
        //                                else
        //                                        Event_post(s, Event_Saturation, State_Succeeded, link.action, "link download saturation check succeeded [current download saturation %.1f%%]", isaturation);
        //                        } else {
        //                                double iosaturation = osaturation + isaturation;
        //                                if (Util_evalDoubleQExpression(link.operator, iosaturation, link.limit))
        //                                        Event_post(s, Event_Saturation, State_Failed, link.action, "link saturation of %.1f%% matches limit [saturation %s %.1f%%]", iosaturation, operatorshortnames[link.operator], link.limit);
        //                                else
        //                                        Event_post(s, Event_Saturation, State_Succeeded, link.action, "link saturation check succeeded [current saturation %.1f%%]", iosaturation);
        //                        }
        //                }
        //        }
        //        // Upload
        //        char buf1[STRLEN], buf2[STRLEN];
        //        for (Bandwidth_T upload = s.uploadbyteslist; upload; upload = upload.next) {
        //                long long obytes;
        //                switch (upload.range) {
        //                        case Time_Minute:
        //                                obytes = Link_getBytesOutPerMinute(s.inf.priv.net.stats, upload.rangecount);
        //                                break;
        //                        case Time_Hour:
        //                                if (upload.rangecount == 1) // Use precise minutes range for "last hour"
        //                                        obytes = Link_getBytesOutPerMinute(s.inf.priv.net.stats, 60);
        //                                else
        //                                        obytes = Link_getBytesOutPerHour(s.inf.priv.net.stats, upload.rangecount);
        //                                break;
        //                        default:
        //                                obytes = Link_getBytesOutPerSecond(s.inf.priv.net.stats);
        //                                break;
        //                }
        //                if (Util_evalQExpression(upload.operator, obytes, upload.limit))
        //                        Event_post(s, Event_ByteOut, State_Failed, upload.action, "%supload %s matches limit [upload rate %s %s in last %d %s]", upload.range != Time_Second ? "total " : "", Str_bytesToSize(obytes, buf1), operatorshortnames[upload.operator], Str_bytesToSize(upload.limit, buf2), upload.rangecount, Util_timestr(upload.range));
        //                else
        //                        Event_post(s, Event_ByteOut, State_Succeeded, upload.action, "%supload check succeeded [current upload rate %s in last %d %s]", upload.range != Time_Second ? "total " : "", Str_bytesToSize(obytes, buf1), upload.rangecount, Util_timestr(upload.range));
        //        }
        //        for (Bandwidth_T upload = s.uploadpacketslist; upload; upload = upload.next) {
        //                long long opackets;
        //                switch (upload.range) {
        //                        case Time_Minute:
        //                                opackets = Link_getPacketsOutPerMinute(s.inf.priv.net.stats, upload.rangecount);
        //                                break;
        //                        case Time_Hour:
        //                                if (upload.rangecount == 1) // Use precise minutes range for "last hour"
        //                                        opackets = Link_getPacketsOutPerMinute(s.inf.priv.net.stats, 60);
        //                                else
        //                                        opackets = Link_getPacketsOutPerHour(s.inf.priv.net.stats, upload.rangecount);
        //                                break;
        //                        default:
        //                                opackets = Link_getPacketsOutPerSecond(s.inf.priv.net.stats);
        //                                break;
        //                }
        //                if (Util_evalQExpression(upload.operator, opackets, upload.limit))
        //                        Event_post(s, Event_PacketOut, State_Failed, upload.action, "%supload packets %lld matches limit [upload packets %s %lld in last %d %s]", upload.range != Time_Second ? "total " : "", opackets, operatorshortnames[upload.operator], upload.limit, upload.rangecount, Util_timestr(upload.range));
        //                else
        //                        Event_post(s, Event_PacketOut, State_Succeeded, upload.action, "%supload packets check succeeded [current upload packets %lld in last %d %s]", upload.range != Time_Second ? "total " : "", opackets, upload.rangecount, Util_timestr(upload.range));
        //        }
        //        // Download
        //        for (Bandwidth_T download = s.downloadbyteslist; download; download = download.next) {
        //                long long ibytes;
        //                switch (download.range) {
        //                        case Time_Minute:
        //                                ibytes = Link_getBytesInPerMinute(s.inf.priv.net.stats, download.rangecount);
        //                                break;
        //                        case Time_Hour:
        //                                if (download.rangecount == 1) // Use precise minutes range for "last hour"
        //                                        ibytes = Link_getBytesInPerMinute(s.inf.priv.net.stats, 60);
        //                                else
        //                                        ibytes = Link_getBytesInPerHour(s.inf.priv.net.stats, download.rangecount);
        //                                break;
        //                        default:
        //                                ibytes = Link_getBytesInPerSecond(s.inf.priv.net.stats);
        //                                break;
        //                }
        //                if (Util_evalQExpression(download.operator, ibytes, download.limit))
        //                        Event_post(s, Event_ByteIn, State_Failed, download.action, "%sdownload %s matches limit [download rate %s %s in last %d %s]", download.range != Time_Second ? "total " : "", Str_bytesToSize(ibytes, buf1), operatorshortnames[download.operator], Str_bytesToSize(download.limit, buf2), download.rangecount, Util_timestr(download.range));
        //                else
        //                        Event_post(s, Event_ByteIn, State_Succeeded, download.action, "%sdownload check succeeded [current download rate %s in last %d %s]", download.range != Time_Second ? "total " : "", Str_bytesToSize(ibytes, buf1), download.rangecount, Util_timestr(download.range));
        //        }
        //        for (Bandwidth_T download = s.downloadpacketslist; download; download = download.next) {
        //                long long ipackets;
        //                switch (download.range) {
        //                        case Time_Minute:
        //                                ipackets = Link_getPacketsInPerMinute(s.inf.priv.net.stats, download.rangecount);
        //                                break;
        //                        case Time_Hour:
        //                                if (download.rangecount == 1) // Use precise minutes range for "last hour"
        //                                        ipackets = Link_getPacketsInPerMinute(s.inf.priv.net.stats, 60);
        //                                else
        //                                        ipackets = Link_getPacketsInPerHour(s.inf.priv.net.stats, download.rangecount);
        //                                break;
        //                        default:
        //                                ipackets = Link_getPacketsInPerSecond(s.inf.priv.net.stats);
        //                                break;
        //                }
        //                if (Util_evalQExpression(download.operator, ipackets, download.limit))
        //                        Event_post(s, Event_PacketIn, State_Failed, download.action, "%sdownload packets %lld matches limit [download packets %s %lld in last %d %s]", download.range != Time_Second ? "total " : "", ipackets, operatorshortnames[download.operator], download.limit, download.rangecount, Util_timestr(download.range));
        //                else
        //                        Event_post(s, Event_PacketIn, State_Succeeded, download.action, "%sdownload packets check succeeded [current download packets %lld in last %d %s]", download.range != Time_Second ? "total " : "", ipackets, download.rangecount, Util_timestr(download.range));
        //        }
        //        return true;
        //}

        #endregion

        #region Private Methods

        private static bool doScheduledAction(Service_T s)
        {
            bool rv = false;
            if (s.doaction != MonitActionType.Action_Ignored)
            {
                // FIXME: let the event engine do the action directly? (just replace s.action_ACTION with s.doaction and drop control_service call)
                rv = Control.ControlService(s.name, s.doaction);
                Event.Post(s, MonitEventType.Event_Action, MonitStateType.State_Changed, s.action_ACTION, "{0} action done", MonitWindowsAgent.actionnames[(int)s.doaction]);
                s.doaction = MonitActionType.Action_Ignored;
                s.token = "";
            }
            return rv;
        }

        private static void checkTimeout(Service_T s)
        {
            if (s.actionratelist == null)
                return;

            /* Start counting cycles */
            if (s.nstart > 0)
                s.ncycle++;

            int max = 0;
            foreach (var ar in s.actionratelist)
            {
                if (max < ar.cycle)
                    max = ar.cycle;
                if (s.nstart >= ar.count && s.ncycle <= ar.cycle)
                    Event.Post(s, MonitEventType.Event_Timeout, MonitStateType.State_Failed, ar.action, "service restarted {0} times within {1} cycles(s) - {2}", s.nstart, s.ncycle, MonitWindowsAgent.actionnames[(int)ar.action.failed.id]);
            }

            /* Stop counting and reset if the cycle interval is succeeded */
            if (s.ncycle > max)
            {
                s.ncycle = 0;
                s.nstart = 0;
            }
        }

        //http://monit.sourcearchive.com/documentation/1:5.3.1-1/Time_8h_a53071867e2c36d7d17289b6e4135a3c3.html
        private static bool _incron(Service_T s, DateTime now)
        {
            if ((now - s.every.last_run) > TimeSpan.FromSeconds(59))
            { // Minute is the lowest resolution, so only run once per minute
                //if(Time_incron(s.every.spec.cron, now)) {
                //    s.every.last_run = now;
                //    return true;
                //}
            }
            return false;
        }

        /// <summary>
        /// Checks if validation should be skiped for this service in this cycle.
        /// </summary>
        /// <param name="s">A service</param>
        /// <returns>true if validation should be skiped for this service in this cycle, otherwise false. Handle every statement</returns>
        private static bool checkSkip(Service_T s)
        {
            if (s.visited)
            {
                Logger.Log.DebugFormat("'{0}' check skipped -- service already handled in a dependency chain", s.name);
                return true;
            }
            DateTime now = DateTime.UtcNow;
            if (s.every.type == MonitEveryType.Every_SkipCycles)
            {
                var cycle = s.every as CycleEvery_T;
                cycle.counter++;
                if (cycle.counter < cycle.number)
                {
                    s.monitor |= MonitMonitorStateType.Monitor_Waiting;
                    Logger.Log.DebugFormat("'{0}' test skipped as current cycle ({1}) < every cycle ({2})", s.name, cycle.counter, cycle.number);
                    return true;
                }
                cycle.counter = 0;
            }
            else if (s.every.type == MonitEveryType.Every_Cron && !_incron(s, now))
            {
                var cron = s.every as CronEvery_T;
                s.monitor |= MonitMonitorStateType.Monitor_Waiting;
                Logger.Log.DebugFormat("'{0}' test skipped as current time ({1}) does not match every's cron spec \"{2}\"", s.name, Timing.GetTimestamp(now), cron.cron);
                return true;
            }
            else if (s.every.type == MonitEveryType.Every_NotInCron)
            {
                var cron = s.every as CronEvery_T;
                if (true/*Time_incron(cron.cron, now)*/)
                {
                    s.monitor |= MonitMonitorStateType.Monitor_Waiting;
                    Logger.Log.DebugFormat("'{0}' test skipped as current time ({1}) matches every's cron spec \"not {2}\"", s.name, Timing.GetTimestamp(now), cron.cron);
                    return true;
                }
            }
            s.monitor &= ~MonitMonitorStateType.Monitor_Waiting;
            return false;
        }

        //        /**
        // * Test the connection and protocol
        // */
        //static void check_connection(Service_T s, Port_T p) {
        //        ASSERT(s && p);
        //        volatile int retry_count = p.retry;
        //        volatile boolean_t rv = true;
        //        char buf[STRLEN];
        //        char report[STRLEN] = {};
        //retry:
        //        TRY
        //        {
        //                Socket_test(p);
        //                DEBUG("'%s' succeeded testing protocol [%s] at %s\n", s.name, p.protocol.name, Util_portDescription(p, buf, sizeof(buf)));
        //        }
        //        ELSE
        //        {
        //                snprintf(report, STRLEN, "failed protocol test [%s] at %s -- %s", p.protocol.name, Util_portDescription(p, buf, sizeof(buf)), Exception_frame.message);
        //                rv = false;
        //        }
        //        END_TRY;
        //        if (! rv) {
        //                if (retry_count-- > 1) {
        //                        DEBUG("'%s' %s (attempt %d/%d)\n", s.name, report, p.retry - retry_count, p.retry);
        //                        goto retry;
        //                }
        //                Event_post(s, Event_Connection, State_Failed, p.action, "%s", report);
        //        } else {
        //                Event_post(s, Event_Connection, State_Succeeded, p.action, "connection succeeded to %s", Util_portDescription(p, buf, sizeof(buf)));
        //        }
        //}


        ///**
        // * Test process state (e.g. Zombie)
        // */
        private static void checkProcessState(Service_T s)
        {
            var process = s.inf as ProcessInfo_T;
            if (process.zombie)
                Event.Post(s, MonitEventType.Event_Data, MonitStateType.State_Failed, s.action_DATA, "process with pid {0} is a zombie", process.pid);
            else
                Event.Post(s, MonitEventType.Event_Data, MonitStateType.State_Succeeded, s.action_DATA, "zombie check succeeded");
        }


        ///**
        // * Test process pid for possible change since last cycle
        // */
        private static void checkProcessPid(Service_T s)
        {
            var process = s.inf as ProcessInfo_T;

            /* process pid was not initialized yet */
            if (process._pid < 0 || process.pid < 0)
                return;

            foreach (var l in s.pidlist)
            {
                if (process._pid != process.pid)
                    Event.Post(s, MonitEventType.Event_Pid, MonitStateType.State_Changed, l.action, "process PID changed from {0} to {1}", process._pid, process.pid);
                else
                    Event.Post(s, MonitEventType.Event_Pid, MonitStateType.State_ChangedNot, l.action, "process PID has not changed since last cycle");
            }
        }


        ///**
        // * Test process ppid for possible change since last cycle
        // */
        private static void checkProcessPpid(Service_T s)
        {

            var process = s.inf as ProcessInfo_T;

            /* process ppid was not initialized yet */
            if (process._ppid < 0 || process.ppid < 0)
                return;

            foreach (var l in s.ppidlist)
            {
                if (process._ppid != process.ppid)
                    Event.Post(s, MonitEventType.Event_PPid, MonitStateType.State_Changed, l.action, "process PPID changed from {0} to {1}", process._ppid, process.ppid);
                else
                    Event.Post(s, MonitEventType.Event_PPid, MonitStateType.State_ChangedNot, l.action, "process PPID has not changed since last cycle");
            }
        }


        /**
         * Check process resources
         */
        private static void checkProcessResources(Service_T s, Resource_T r)
        {
            bool okay = true;
            string report = "";
            var process = s.inf as ProcessInfo_T;

            switch (r.resource_id)
            {
                case MonitResourceType.Resource_CpuPercent:
                    if (s.monitor == MonitMonitorStateType.Monitor_Init || process.cpu_percent < 0)
                    {
                        Logger.Log.DebugFormat("'{0}' cpu usage check skipped (initializing)", s.name);
                        return;
                    }
                    else if (Util.EvalQExpression(r.compOperator, process.cpu_percent, r.limit))
                    {
                        report += string.Format("cpu usage of {0}% matches resource limit [cpu usage{1}{2}%]", process.cpu_percent / 10.0, r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("cpu usage check succeeded [current cpu usage={0}%]", process.cpu_percent / 10.0);
                    break;
                case MonitResourceType.Resource_CpuPercentTotal:
                    if (s.monitor == MonitMonitorStateType.Monitor_Init || process.total_cpu_percent < 0)
                    {
                        Logger.Log.DebugFormat("'{0}' total cpu usage check skipped (initializing)", s.name);
                        return;
                    }
                    else if (Util.EvalQExpression(r.compOperator, process.total_cpu_percent, r.limit))
                    {
                        report += string.Format("total cpu usage of {0}% matches resource limit [cpu usage{1}{2}%]", process.total_cpu_percent / 10.0, r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("total cpu usage check succeeded [current cpu usage={0}%]", process.total_cpu_percent / 10.0);
                    break;

                case MonitResourceType.Resource_CpuUser:
                    if (s.monitor == MonitMonitorStateType.Monitor_Init || MonitWindowsAgent.systeminfo.total_cpu_user_percent < 0)
                    {
                        Logger.Log.DebugFormat("'{0}' cpu user usage check skipped (initializing)", s.name);
                        return;
                    }
                    else if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_cpu_user_percent, r.limit))
                    {
                        report += string.Format("cpu user usage of %.1f%% matches resource limit [cpu user usage%s%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_user_percent / 10.0, r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("cpu user usage check succeeded [current cpu user usage=%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_user_percent / 10.0);
                    break;

                case MonitResourceType.Resource_CpuSystem:
                    if (s.monitor == MonitMonitorStateType.Monitor_Init || MonitWindowsAgent.systeminfo.total_cpu_syst_percent < 0)
                    {
                        Logger.Log.DebugFormat("'{0}' cpu system usage check skipped (initializing)", s.name);
                        return;
                    }
                    else if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_cpu_syst_percent, r.limit))
                    {
                        report += string.Format("cpu system usage of %.1f%% matches resource limit [cpu system usage%s%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_syst_percent / 10.0, r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("cpu system usage check succeeded [current cpu system usage=%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_syst_percent / 10.0);
                    break;

                case MonitResourceType.Resource_CpuWait:
                    if (s.monitor == MonitMonitorStateType.Monitor_Init || MonitWindowsAgent.systeminfo.total_cpu_wait_percent < 0)
                    {
                        Logger.Log.DebugFormat("'{0}' cpu wait usage check skipped (initializing)", s.name);
                        return;
                    }
                    else if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_cpu_wait_percent, r.limit))
                    {
                        report += string.Format("cpu wait usage of %.1f%% matches resource limit [cpu wait usage%s%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_wait_percent / 10.0, r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("cpu wait usage check succeeded [current cpu wait usage=%.1f%%]", MonitWindowsAgent.systeminfo.total_cpu_wait_percent / 10.0);
                    break;

                case MonitResourceType.Resource_MemoryPercent:
                    if (s.type == MonitServiceType.Service_System)
                    {
                        if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_mem_percent, r.limit))
                        {
                            report += string.Format("mem usage of {0}% matches resource limit [mem usage{1}{2}%]", MonitWindowsAgent.systeminfo.total_mem_percent / 10.0, r.compOperator, r.limit / 10.0);
                            okay = false;
                        }
                        else
                            report += string.Format("mem usage check succeeded [current mem usage={0}%]", MonitWindowsAgent.systeminfo.total_mem_percent / 10.0);
                    }
                    else
                    {
                        if (Util.EvalQExpression(r.compOperator, process.mem_percent, r.limit))
                        {
                            report += string.Format("mem usage of {0}% matches resource limit [mem usage{1}{2}%]", process.mem_percent / 10.0, r.compOperator, r.limit / 10.0);
                            okay = false;
                        }
                        else
                            report += string.Format("mem usage check succeeded [current mem usage={0}%]", process.mem_percent / 10.0);
                    }
                    break;

                case MonitResourceType.Resource_MemoryKbyte:
                    if (s.type == MonitServiceType.Service_System)
                    {
                        if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_mem_kbyte, r.limit))
                        {
                            report += string.Format("mem amount of %s matches resource limit [mem amount%s%s]", MonitWindowsAgent.systeminfo.total_mem_kbyte * 1024f, r.compOperator, r.limit * 1024f);
                            okay = false;
                        }
                        else
                            report += string.Format("mem amount check succeeded [current mem amount=%s]", MonitWindowsAgent.systeminfo.total_mem_kbyte * 1024f);
                    }
                    else
                    {
                        if (Util.EvalQExpression(r.compOperator, process.mem_kbyte, r.limit))
                        {
                            report += string.Format("mem amount of %s matches resource limit [mem amount%s%s]", process.mem_kbyte * 1024f, r.compOperator, r.limit * 1024f);
                            okay = false;
                        }
                        else
                            report += string.Format("mem amount check succeeded [current mem amount=%s]", process.mem_kbyte * 1024f);
                    }
                    break;

                case MonitResourceType.Resource_SwapPercent:
                    if (s.type == MonitServiceType.Service_System)
                    {
                        if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_swap_percent, r.limit))
                        {
                            report += string.Format("swap usage of %.1f%% matches resource limit [swap usage%s%.1f%%]", MonitWindowsAgent.systeminfo.total_swap_percent / 10.0, r.compOperator, r.limit / 10.0);
                            okay = false;
                        }
                        else
                            report += string.Format("swap usage check succeeded [current swap usage=%.1f%%]", MonitWindowsAgent.systeminfo.total_swap_percent / 10.0);
                    }
                    break;

                case MonitResourceType.Resource_SwapKbyte:
                    if (s.type == MonitServiceType.Service_System)
                    {
                        if (Util.EvalQExpression(r.compOperator, MonitWindowsAgent.systeminfo.total_swap_kbyte, r.limit))
                        {
                            report += string.Format("swap amount of %s matches resource limit [swap amount%s%s]", MonitWindowsAgent.systeminfo.total_swap_kbyte * 1024f, r.compOperator, r.limit * 1024f);
                            okay = false;
                        }
                        else
                            report += string.Format("swap amount check succeeded [current swap amount=%s]", MonitWindowsAgent.systeminfo.total_swap_kbyte * 1024f);
                    }
                    break;

                case MonitResourceType.Resource_LoadAverage1m:
                    if (Util.EvalQExpression(r.compOperator, (int)(MonitWindowsAgent.systeminfo.loadavg[0] * 10.0), r.limit))
                    {
                        report += string.Format("loadavg(1min) of %.1f matches resource limit [loadavg(1min)%s%.1f]", MonitWindowsAgent.systeminfo.loadavg[0], r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("loadavg(1min) check succeeded [current loadavg(1min)=%.1f]", MonitWindowsAgent.systeminfo.loadavg[0]);
                    break;

                case MonitResourceType.Resource_LoadAverage5m:
                    if (Util.EvalQExpression(r.compOperator, (int)(MonitWindowsAgent.systeminfo.loadavg[1] * 10.0), r.limit))
                    {
                        report += string.Format("loadavg(5min) of %.1f matches resource limit [loadavg(5min)%s%.1f]", MonitWindowsAgent.systeminfo.loadavg[1], r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("loadavg(5min) check succeeded [current loadavg(5min)=%.1f]", MonitWindowsAgent.systeminfo.loadavg[1]);
                    break;

                case MonitResourceType.Resource_LoadAverage15m:
                    if (Util.EvalQExpression(r.compOperator, (int)(MonitWindowsAgent.systeminfo.loadavg[2] * 10.0), r.limit))
                    {
                        report += string.Format("loadavg(15min) of %.1f matches resource limit [loadavg(15min)%s%.1f]", MonitWindowsAgent.systeminfo.loadavg[2], r.compOperator, r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("loadavg(15min) check succeeded [current loadavg(15min)=%.1f]", MonitWindowsAgent.systeminfo.loadavg[2]);
                    break;

                case MonitResourceType.Resource_Children:
                    if (Util.EvalQExpression(r.compOperator, process.children, r.limit))
                    {
                        report += string.Format("children of %i matches resource limit [children%s%ld]", process.children, r.compOperator, r.limit);
                        okay = false;
                    }
                    else
                        report += string.Format("children check succeeded [current children=%i]", process.children);
                    break;

                case MonitResourceType.Resource_MemoryKbyteTotal:
                    if (Util.EvalQExpression(r.compOperator, process.total_mem_kbyte, r.limit))
                    {
                        report += string.Format("total mem amount of {0} matches resource limit [total mem amount{1}{2}]", process.total_mem_kbyte * 1024f, r.compOperator, r.limit * 1024f);
                        okay = false;
                    }
                    else
                        report += string.Format("total mem amount check succeeded [current total mem amount={0}]", process.total_mem_kbyte * 1024f);
                    break;

                case MonitResourceType.Resource_MemoryPercentTotal:
                    if (Util.EvalQExpression(r.compOperator, process.total_mem_percent, r.limit))
                    {
                        report += string.Format("total mem amount of {0}% matches resource limit [total mem amount{1}{2}%]", (float)process.total_mem_percent / 10.0, r.compOperator, (float)r.limit / 10.0);
                        okay = false;
                    }
                    else
                        report += string.Format("total mem amount check succeeded [current total mem amount={0}%]", process.total_mem_percent / 10.0);
                    break;

                default:
                    Logger.Log.ErrorFormat("'{0}' error -- unknown resource ID: [{1}]", s.name, r.resource_id);
                    return;
            }
            Event.Post(s, MonitEventType.Event_Resource, okay ? MonitStateType.State_Succeeded : MonitStateType.State_Failed, r.action, report);
        }


        ///**
        // * Test for associated path checksum change
        // */
        //static void check_checksum(Service_T s) {
        //        int         changed;
        //        Checksum_T  cs;

        //        ASSERT(s && s.path && s.checksum);

        //        cs = s.checksum;

        //        if (Util_getChecksum(s.path, cs.type, s.inf.priv.file.cs_sum, sizeof(s.inf.priv.file.cs_sum))) {

        //                Event_post(s, Event_Data, State_Succeeded, s.action_DATA, "checksum computed for %s", s.path);

        //                if (! cs.initialized) {
        //                        cs.initialized = true;
        //                        snprintf(cs.hash, sizeof(cs.hash), "%s", s.inf.priv.file.cs_sum);
        //                }

        //                switch (cs.type) {
        //                        case Hash_Md5:
        //                                changed = strncmp(cs.hash, s.inf.priv.file.cs_sum, 32);
        //                                break;
        //                        case Hash_Sha1:
        //                                changed = strncmp(cs.hash, s.inf.priv.file.cs_sum, 40);
        //                                break;
        //                        default:
        //                                LogError("'%s' unknown hash type\n", s.name);
        //                                *s.inf.priv.file.cs_sum = 0;
        //                                return;
        //                }

        //                if (changed) {

        //                        if (cs.test_changes) {
        //                                /* if we are testing for changes only, the value is variable */
        //                                Event_post(s, Event_Checksum, State_Changed, cs.action, "checksum was changed for %s", s.path);
        //                                /* reset expected value for next cycle */
        //                                snprintf(cs.hash, sizeof(cs.hash), "%s", s.inf.priv.file.cs_sum);
        //                        } else {
        //                                /* we are testing constant value for failed or succeeded state */
        //                                Event_post(s, Event_Checksum, State_Failed, cs.action, "checksum test failed for %s", s.path);
        //                        }

        //                } else if (cs.test_changes) {
        //                        Event_post(s, Event_Checksum, State_ChangedNot, cs.action, "checksum has not changed");
        //                } else {
        //                        Event_post(s, Event_Checksum, State_Succeeded, cs.action, "checksum is valid");
        //                }
        //                return;
        //        }

        //        Event_post(s, Event_Data, State_Failed, s.action_DATA, "cannot compute checksum for %s", s.path);

        //}


        ///**
        // * Test for associated path permission change
        // */
        //static void check_perm(Service_T s, mode_t mode) {
        //        ASSERT(s && s.perm);
        //        mode_t m = mode & 07777;
        //        if (m != s.perm.perm) {
        //                if (s.perm.test_changes) {
        //                        Event_post(s, Event_Permission, State_Changed, s.perm.action, "permission for %s changed from %04o to %04o", s.path, s.perm.perm, m);
        //                        s.perm.perm = m;
        //                } else {
        //                        Event_post(s, Event_Permission, State_Failed, s.perm.action, "permission test failed for %s [current permission %04o]", s.path, m);
        //                }
        //        } else {
        //                if (s.perm.test_changes)
        //                        Event_post(s, Event_Permission, State_ChangedNot, s.perm.action, "permission not changed for %s", s.path);
        //                else
        //                        Event_post(s, Event_Permission, State_Succeeded, s.perm.action, "permission test succeeded [current permission %04o]", m);
        //        }
        //}


        ///**
        // * Test UID of file or process
        // */
        static void checkUid(Service_T s, int uid)
        {
            if (uid >= 0)
            {
                if (uid != s.uid.uid)
                    Event.Post(s, MonitEventType.Event_Uid, MonitStateType.State_Failed, s.uid.action, "uid test failed for {0} -- current uid is {1}", s.name, uid);
                else
                    Event.Post(s, MonitEventType.Event_Uid, MonitStateType.State_Succeeded, s.uid.action, "uid test succeeded [current uid={0}]", uid);
            }
        }


        ///**
        // * Test effective UID of process
        // */
        private static void checkEuid(Service_T s, int euid)
        {
            if (euid >= 0)
            {
                if (euid != s.euid.uid)
                    Event.Post(s, MonitEventType.Event_Uid, MonitStateType.State_Failed, s.euid.action, "euid test failed for {0} -- current euid is {1}", s.name, euid);
                else
                    Event.Post(s, MonitEventType.Event_Uid, MonitStateType.State_Succeeded, s.euid.action, "euid test succeeded [current euid={0}]", euid);
            }
        }


        ///**
        // * Test GID of file or process
        // */
        private static void checkGid(Service_T s, int gid)
        {
            if (gid >= 0)
            {
                if (gid != s.gid.gid)
                    Event.Post(s, MonitEventType.Event_Gid, MonitStateType.State_Failed, s.gid.action, "gid test failed for {0} -- current gid is {1}", s.name, gid);
                else
                    Event.Post(s, MonitEventType.Event_Gid, MonitStateType.State_Succeeded, s.gid.action, "gid test succeeded [current gid={0}]", gid);
            }
        }


        ///**
        // * Validate timestamps of a service s
        // */
        private static void checkTimestamp(Service_T s, DateTime timestamp)
        {
            Event.Post(s, MonitEventType.Event_Data, MonitStateType.State_Succeeded, s.action_DATA, "actual system time obtained");

            foreach (var t in s.timestamplist)
            {
                if (t.test_changes)
                {
                    /* if we are testing for changes only, the value is variable */
                    if (t.timestamp != timestamp)
                    {
                        /* reset expected value for next cycle */
                        t.timestamp = timestamp;
                        Event.Post(s, MonitEventType.Event_Timestamp, MonitStateType.State_Changed, t.action, "timestamp was changed for {0}", s.path);
                    }
                    else
                    {
                        Event.Post(s, MonitEventType.Event_Timestamp, MonitStateType.State_ChangedNot, t.action, "timestamp was not changed for {0}", s.path);
                    }
                }
                else
                {
                    /* we are testing constant value for failed or succeeded state */
                    if (Util.EvalQExpression(t.compOperator, (DateTime.Now - timestamp), t.time))
                        Event.Post(s, MonitEventType.Event_Timestamp, MonitStateType.State_Failed, t.action, "timestamp test failed for {0}", s.path);
                    else
                        Event.Post(s, MonitEventType.Event_Timestamp, MonitStateType.State_Succeeded, t.action, "timestamp test succeeded for {0}", s.path);
                }
            }
        }


        ///**
        // * Test size
        // */
        //static void check_size(Service_T s) {
        //        ASSERT(s && s.sizelist);
        //        char buf[10];
        //        for (Size_T sl = s.sizelist; sl; sl = sl.next) {
        //                /* if we are testing for changes only, the value is variable */
        //                if (sl.test_changes) {
        //                        if (! sl.initialized) {
        //                                /* the size was not initialized during monit start, so set the size now
        //                                 * and allow further size change testing */
        //                                sl.initialized = true;
        //                                sl.size = s.inf.priv.file.size;
        //                        } else {
        //                                if (sl.size != s.inf.priv.file.size) {
        //                                        Event_post(s, Event_Size, State_Changed, sl.action, "size was changed for %s", s.path);
        //                                        /* reset expected value for next cycle */
        //                                        sl.size = s.inf.priv.file.size;
        //                                } else {
        //                                        Event_post(s, Event_Size, State_ChangedNot, sl.action, "size has not changed [current size=%s]", Str_bytesToSize(s.inf.priv.file.size, buf));
        //                                }
        //                        }
        //                } else {
        //                        /* we are testing constant value for failed or succeeded state */
        //                        if (Util_evalQExpression(sl.operator, s.inf.priv.file.size, sl.size))
        //                                Event_post(s, Event_Size, State_Failed, sl.action, "size test failed for %s -- current size is %s", s.path, Str_bytesToSize(s.inf.priv.file.size, buf));
        //                        else
        //                                Event_post(s, Event_Size, State_Succeeded, sl.action, "size check succeeded [current size=%s]", Str_bytesToSize(s.inf.priv.file.size, buf));
        //                }
        //        }
        //}


        ///**
        // * Test uptime
        // */
        private static void checkUptime(Service_T s) {
            var process = s.inf as ProcessInfo_T;

            foreach (var ul in s.uptimelist) {
                if (Util.EvalQExpression(ul.compOperator, process.uptime, ul.uptime))
                    Event.Post(s, MonitEventType.Event_Uptime, MonitStateType.State_Failed, ul.action, "uptime test failed for {0} -- current uptime is {1} seconds", s.path, process.uptime);
                else
                    Event.Post(s, MonitEventType.Event_Uptime, MonitStateType.State_Succeeded, ul.action, "uptime test succeeded [current uptime={0} seconds]", process.uptime);
            }
        }


        //static int check_pattern(Match_T pattern, const char *line) {
        //#ifdef HAVE_REGEX_H
        //        return regexec(pattern.regex_comp, line, 0, NULL, 0);
        //#else
        //        if (strstr(line, pattern.match_string) == NULL)
        //                return -1;
        //        else
        //                return 0;
        //#endif
        //}


        ///**
        // * Match content.
        // *
        // * The test compares only the lines terminated with \n.
        // *
        // * In the case that line with missing \n is read, the test stops, as we suppose that the file contains only partial line and the rest of it is yet stored in the buffer of the application which writes to the file.
        // * The test will resume at the beginning of the incomplete line during the next cycle, allowing the writer to finish the write.
        // *
        // * We test only MATCH_LINE_LENGTH at maximum (512 bytes) - in the case that the line is bigger, we read the rest of the line (till '\n') but ignore the characters past the maximum (512+).
        // */
        //static void check_match(Service_T s) {
        //        Match_T ml;
        //        FILE *file;
        //        char line[MATCH_LINE_LENGTH];

        //        ASSERT(s && s.matchlist);

        //        /* Open the file */
        //        if (! (file = fopen(s.path, "r"))) {
        //                LogError("'%s' cannot open file %s: %s\n", s.name, s.path, STRERROR);
        //                return;
        //        }

        //        /* FIXME: Refactor: Initialize the filesystems table ahead of file and filesystems test and index it by device id + replace the Str_startsWith() with lookup to the table by device id (obtained via file's stat()).
        //         The central filesystems initialization will allow to reduce the statfs() calls in the case that there will be multiple file and/or filesystems tests for the same fs. Temporarily we go with
        //         dummy Str_startsWith() as quick fix which will cover 99.9% of use cases without rising the statfs overhead if statfs call would be inlined here.
        //         */
        //        if (Str_startsWith(s.path, "/proc")) {
        //                s.inf.priv.file.readpos = 0;
        //        } else {
        //                /* If inode changed or size shrinked . set read position = 0 */
        //                if (s.inf.priv.file.inode != s.inf.priv.file.inode_prev || s.inf.priv.file.readpos > s.inf.priv.file.size)
        //                        s.inf.priv.file.readpos = 0;

        //                /* Do we need to match? Even if not, go to final, so we can reset the content match error flags in this cycle */
        //                if (s.inf.priv.file.readpos == s.inf.priv.file.size) {
        //                        DEBUG("'%s' content match skipped - file size nor inode has not changed since last test\n", s.name);
        //                        goto final;
        //                }
        //        }

        //        while (true) {
        //        next:
        //                /* Seek to the read position */
        //                if (fseek(file, (long)s.inf.priv.file.readpos, SEEK_SET)) {
        //                        LogError("'%s' cannot seek file %s: %s\n", s.name, s.path, STRERROR);
        //                        goto final;
        //                }

        //                if (! fgets(line, MATCH_LINE_LENGTH, file)) {
        //                        if (! feof(file))
        //                                LogError("'%s' cannot read file %s: %s\n", s.name, s.path, STRERROR);
        //                        goto final;
        //                }

        //                size_t length = strlen(line);
        //                if (length == 0) {
        //                        /* No content: shouldn't happen - empty line will contain at least '\n' */
        //                        goto final;
        //                } else if (line[length-1] != '\n') {
        //                        if (length < MATCH_LINE_LENGTH-1) {
        //                                /* Incomplete line: we gonna read it next time again, allowing the writer to complete the write */
        //                                DEBUG("'%s' content match: incomplete line read - no new line at end. (retrying next cycle)\n", s.name);
        //                                goto final;
        //                        } else if (length == MATCH_LINE_LENGTH-1) {
        //                                /* Our read buffer is full: ignore the content past the MATCH_LINE_LENGTH */
        //                                int rv;
        //                                do {
        //                                        if ((rv = fgetc(file)) == EOF)
        //                                                goto final;
        //                                        length++;
        //                                } while (rv != '\n');
        //                        }
        //                } else {
        //                        /* Remove appending newline */
        //                        line[length - 1] = 0;
        //                }
        //                /* Set read position to the end of last read */
        //                s.inf.priv.file.readpos += length;

        //                /* Check ignores */
        //                for (ml = s.matchignorelist; ml; ml = ml.next) {
        //                        if ((check_pattern(ml, line) == 0)  ^ (ml.not)) {
        //                                /* We match! . line is ignored! */
        //                                DEBUG("'%s' Ignore pattern %s'%s' match on content line\n", s.name, ml.not ? "not " : "", ml.match_string);
        //                                goto next;
        //                        }
        //                }

        //                /* Check non ignores */
        //                for (ml = s.matchlist; ml; ml = ml.next) {
        //                        if ((check_pattern(ml, line) == 0) ^ (ml.not)) {
        //                                DEBUG("'%s' Pattern %s'%s' match on content line [%s]\n", s.name, ml.not ? "not " : "", ml.match_string, line);
        //                                /* Save the line: we limit the content showed in the event roughly to MATCH_LINE_LENGTH (we allow exceed to not break the line) */
        //                                if (! ml.log)
        //                                        ml.log = StringBuffer_create(MATCH_LINE_LENGTH);
        //                                if (StringBuffer_length(ml.log) < MATCH_LINE_LENGTH) {
        //                                        StringBuffer_append(ml.log, "%s\n", line);
        //                                        if (StringBuffer_length(ml.log) >= MATCH_LINE_LENGTH)
        //                                                StringBuffer_append(ml.log, "...\n");
        //                                }
        //                        } else {
        //                                DEBUG("'%s' Pattern %s'%s' doesn't match on content line [%s]\n", s.name, ml.not ? "not " : "", ml.match_string, line);
        //                        }
        //                }
        //        }
        //final:
        //        if (fclose(file))
        //                LogError("'%s' cannot close file %s: %s\n", s.name, s.path, STRERROR);

        //        /* Post process the matches: generate events for particular patterns */
        //        for (ml = s.matchlist; ml; ml = ml.next) {
        //                if (ml.log) {
        //                        Event_post(s, Event_Content, State_Changed, ml.action, "content match:\n%s", StringBuffer_toString(ml.log));
        //                        StringBuffer_free(&ml.log);
        //                } else {
        //                        Event_post(s, Event_Content, State_ChangedNot, ml.action, "content doesn't match");
        //                }
        //        }
        //}


        ///**
        // * Test filesystem flags for possible change since last cycle
        // */
        //static void check_filesystem_flags(Service_T s) {
        //        ASSERT(s && s.inf);

        //        /* filesystem flags were not initialized yet */
        //        if (s.inf.priv.filesystem._flags == -1)
        //                return;

        //        for (Fsflag_T l = s.fsflaglist; l; l = l.next)
        //                if (s.inf.priv.filesystem._flags != s.inf.priv.filesystem.flags)
        //                        Event_post(s, Event_Fsflag, State_Changed, l.action, "filesytem flags changed to %#x", s.inf.priv.filesystem.flags);
        //}

        /**
         * Filesystem test
         */
        private static void checkFilesystemResources(Service_T s, Filesystem_T td)
        {
            if ((td.limit_percent < 0) && (td.limit_absolute < 0))
            {
                Logger.Log.ErrorFormat("'{0}' error: filesystem limit not set", s.name);
                return;
            }

            var filesystem = s.inf as FileSystemInfo_T;

            switch (td.resource)
            {
                case MonitResourceType.Resource_Inode:
                        if (filesystem.f_files <= 0) {
                                Logger.Log.DebugFormat("'{0}' filesystem doesn't support inodes", s.name);
                                return;
                        }

                        if (td.limit_percent >= 0) {
                                if (Util.EvalQExpression(td.compOperator, filesystem.inode_percent, td.limit_percent)) {
                                        Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Failed, td.action, "inode usage {0}% matches resource limit [inode usage{1}{2}%]", filesystem.inode_percent/10f, td.compOperator, td.limit_percent/10f);
                                        return;
                                }
                        } else {
                                if (Util.EvalQExpression(td.compOperator, filesystem.inode_total, td.limit_absolute)) {
                                        Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Failed, td.action, "inode usage {0} matches resource limit [inode usage{1}{2}]", filesystem.inode_total, td.compOperator, td.limit_absolute);
                                        return;
                                }
                        }
                        Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Succeeded, td.action, "inode usage test succeeded [current inode usage={0}%]", filesystem.inode_percent/10f);
                        return;
                case MonitResourceType.Resource_Space:
                    if (td.limit_percent >= 0) {
                            if (Util.EvalQExpression(td.compOperator, filesystem.space_percent, td.limit_percent)) {
                                    Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Failed, td.action, "space usage {0}% matches resource limit [space usage{1}{2}%]", filesystem.space_percent/10f, td.compOperator, td.limit_percent/10f);
                                    return;
                            }
                    } else {
                            if (Util.EvalQExpression(td.compOperator, filesystem.space_total, td.limit_absolute)) {
                                    if (filesystem.f_bsize > 0) {
                                            Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Failed, td.action, "space usage {0} matches resource limit [space usage{1}{2}]", filesystem.space_total * filesystem.f_bsize, td.compOperator, td.limit_absolute * filesystem.f_bsize);
                                    } else {
                                            Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Failed, td.action, "space usage {0} blocks matches resource limit [space usage{1}{2} blocks]", filesystem.space_total, td.compOperator, td.limit_absolute);
                                    }
                                    return;
                            }
                    }
                    Event.Post(s, MonitEventType.Event_Resource, MonitStateType.State_Succeeded, td.action, "space usage test succeeded [current space usage={0}%]", filesystem.space_percent / 10f);
                    return;

                default:
                    Logger.Log.ErrorFormat("'{0}' error -- unknown resource type: [{1}]", s.name, td.resource);
                    return;
            }
        }

        #endregion
    }
}
