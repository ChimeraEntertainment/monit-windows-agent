using System;
using System.ServiceProcess;
using System.Text;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;

namespace ChMonitoring
{
    /// <summary>
    ///     TODO
    /// </summary>
    internal class Util
    {
        #region Public Methods

        public static Service_T GetService(string name)
        {
            Service_T S = null;
            MonitWindowsAgent.servicelist.ForEach(s =>
            {
                if (s.name == name)
                    S = s;
            });
            return S;
        }

        public static bool ExistService(string name)
        {
            return GetService(name) != null;
        }

        public static MonitActionType GetAction(string action)
        {
            for (var i = 1; i < MonitWindowsAgent.actionnames.Length; i++)
            {
                if (action == MonitWindowsAgent.actionnames[i])
                    return (MonitActionType) i;
            }

            /* the action was not found */
            return MonitActionType.Action_Ignored;
        }

        public static bool HasServiceStatus(Service_T S)
        {
            return (S.monitor == MonitMonitorStateType.Monitor_Yes &&
                    !Event.IsEventSet(S.error, MonitEventType.Event_Nonexist) &&
                    !Event.IsEventSet(S.error, MonitEventType.Event_Data));
        }

        public static int IsProcessRunning(Service_T s, bool refresh)
        {
            //TODO
            return ProcessHelper.GetController(s.name).Status == ServiceControllerStatus.Running ? 1 : 0;

            var pid = -1;
            //        if (s.matchlist) {
            //                if (refresh || ! ptree || ! ptreesize)
            //                        initprocesstree(&ptree, &ptreesize, &oldptree, &oldptreesize);
            //                /* The process table read may sporadically fail during read, because we're using glob on some platforms which may fail if the proc filesystem
            //                 * which it traverses is changed during glob (process stopped). Note that the glob failure is rare and temporary - it will be OK on next cycle.
            //                 * We skip the process matching that cycle however because we don't have process informations - will retry next cycle */
            //                if (Run.doprocess) {
            //                        for (int i = 0; i < ptreesize; i++) {
            //                                boolean_t found = false;
            //                                if (ptree[i].cmdline) {
            //#ifdef HAVE_REGEX_H
            //                                        found = regexec(s.matchlist.regex_comp, ptree[i].cmdline, 0, NULL, 0) ? false : true;
            //#else
            //                                        found = strstr(ptree[i].cmdline, s.matchlist.match_string) ? true : false;
            //#endif
            //                                }
            //                                if (found) {
            //                                        pid = ptree[i].pid;
            //                                        break;
            //                                }
            //                        }
            //                } else {
            //                        DEBUG("Process information not available -- skipping service %s process existence check for this cycle\n", s.name);
            //                        /* Return value is NOOP - it is based on existing errors bitmap so we don't generate false recovery/failures */
            //                        return ! (s.error & Event_Nonexist);
            //                }
            //        } else {
            pid = GetPid(s.path);
            //}
            if (pid > 0)
            {
                //if (getpgid(pid) > -1)
                return pid;
                //ChMonitoring.Helpers.Logger.Log.DebugFormat("'{0}' process test failed [pid={1}]", s.name, pid);
            }
            ResetInfo(s);
            return 0;
        }

        public static int GetPid(string pidfile)
        {
            return 1;
            //FILE* file = NULL;
            //int pid = -1;

            //ASSERT(pidfile);

            //if (!File_exist(pidfile))
            //{
            //    DEBUG("pidfile '%s' does not exist\n", pidfile);
            //    return 0;
            //}
            //if (!File_isFile(pidfile))
            //{
            //    LogError("pidfile '%s' is not a regular file\n", pidfile);
            //    return 0;
            //}
            //if ((file = fopen(pidfile, "r")) == (FILE*)NULL)
            //{
            //    LogError("Error opening the pidfile '%s' -- %s\n", pidfile, STRERROR);
            //    return 0;
            //}
            //if (fscanf(file, "%d", &pid) != 1)
            //{
            //    LogError("Error reading pid from file '%s'\n", pidfile);
            //    if (fclose(file))
            //        LogError("Error closing file '%s' -- %s\n", pidfile, STRERROR);
            //    return 0;
            //}
            //if (fclose(file))
            //    LogError("Error closing file '%s' -- %s\n", pidfile, STRERROR);

            //if (pid < 0)
            //    return (0);

            //return (pid_t)pid;
        }

        public static void ResetInfo(Service_T s)
        {
            switch (s.type)
            {
                case MonitServiceType.Service_Filesystem:
                    var filesystem = s.inf as FileSystemInfo_T;
                    filesystem.f_bsize = 0;
                    filesystem.f_blocks = 0;
                    filesystem.f_blocksfree = 0;
                    filesystem.f_blocksfreetotal = 0;
                    filesystem.f_files = 0;
                    filesystem.f_filesfree = 0;
                    filesystem.inode_percent = 0;
                    filesystem.inode_total = 0;
                    filesystem.space_percent = 0;
                    filesystem.space_total = 0;
                    filesystem._flags = -1;
                    filesystem.flags = -1;
                    filesystem.mode = 0;
                    filesystem.uid = 0;
                    filesystem.gid = 0;
                    break;
                case MonitServiceType.Service_File:
                    var file = s.inf as FileInfo_T;
                    // persistent: st_inode, readpos
                    file.size = 0;
                    file.inode_prev = 0;
                    file.mode = 0;
                    file.uid = 0;
                    file.gid = 0;
                    file.timestamp = DateTime.MinValue;
                    file.cs_sum = "";
                    break;
                case MonitServiceType.Service_Directory:
                    var directory = s.inf as DirectoryInfo_T;
                    directory.mode = 0;
                    directory.uid = 0;
                    directory.gid = 0;
                    directory.timestamp = DateTime.MinValue;
                    break;
                case MonitServiceType.Service_Fifo:
                    var fifo = s.inf as FiFoInfo_T;
                    fifo.mode = 0;
                    fifo.uid = 0;
                    fifo.gid = 0;
                    fifo.timestamp = DateTime.MinValue;
                    break;
                case MonitServiceType.Service_Process:
                    var process = s.inf as ProcessInfo_T;
                    process._pid = -1;
                    process._ppid = -1;
                    process.pid = -1;
                    process.ppid = -1;
                    process.uid = -1;
                    process.euid = -1;
                    process.gid = -1;
                    process.zombie = false;
                    process.children = 0;
                    process.mem_kbyte = 0L;
                    process.total_mem_kbyte = 0L;
                    process.mem_percent = 0;
                    process.total_mem_percent = 0;
                    process.cpu_percent = 0;
                    process.total_cpu_percent = 0;
                    process.uptime = TimeSpan.MinValue;
                    break;
                case MonitServiceType.Service_Net:
                    var net = s.inf as NetInfo_T;
                    //if (net.stats)
                    //        Link_reset(net.stats);
                    break;
                default:
                    break;
            }
        }

        public static void MonitorSet(Service_T s)
        {
            if (s.monitor == MonitMonitorStateType.Monitor_Not)
            {
                s.monitor = MonitMonitorStateType.Monitor_Init;
                Logger.Log.DebugFormat("'{0}' monitoring enabled", s.name);
            }
        }


        public static void MonitorUnset(Service_T s)
        {
            if (s.monitor != MonitMonitorStateType.Monitor_Not)
            {
                s.monitor = MonitMonitorStateType.Monitor_Not;
                Logger.Log.DebugFormat("'{0}' monitoring disabled", s.name);
            }
            s.nstart = 0;
            s.ncycle = 0;
            if (s.every.type == MonitEveryType.Every_SkipCycles)
            {
                var cycle = s.every as CycleEvery_T;
                cycle.counter = 0;
            }
            s.error = (int) MonitEventType.Event_Null;
            //if (s.eventlist)
            //    s.eventlist = null;
            ResetInfo(s);
        }

        public static void PrintRule(StringBuilder buf, EventAction_T action, string rule, params object[] args)
        {
            buf.AppendFormat(rule, args);
            // Constant part (failure action)
            buf.Append(" ");
            PrintEventratio(action.failed, buf);
            buf.Append("then ");
            PrintAction(action.failed, buf);
            // Print the success part only if it's non default action (alert is implicit => skipped for simpler output)
            if (action.succeeded.id != MonitActionType.Action_Ignored &&
                action.succeeded.id != MonitActionType.Action_Alert)
            {
                buf.Append(" else if succeeded ");
                PrintEventratio(action.succeeded, buf);
                buf.Append("then ");
                PrintAction(action.succeeded, buf);
            }
        }

        public static void PrintAction(Action_T A, StringBuilder buf)
        {
            buf.Append(MonitWindowsAgent.actionnames[(int) A.id]);
            if (A.id == MonitActionType.Action_Exec)
            {
                buf.Append(A.exec.Method.Name);
                //var C = A.exec;
                //for (int i = 0; C.arg[i]; i++)
                //        buf.AppendFormat("{0}{1}", i != 0 ? " " : " '", C.arg[i]);
                //buf.Append("'");
                //if (C.has_uid)
                //        buf.AppendFormat(" as uid {0}", C.uid);
                //if (C.has_gid)
                //        buf.AppendFormat(" as gid {0}", C.gid);
                //if (C.timeout)
                //        buf.AppendFormat(" timeout {0} cycle(s)", C.timeout);
            }
        }

        public static void PrintEventratio(Action_T action, StringBuilder buf)
        {
            if (action.cycles > 1)
            {
                if (action.count == action.cycles)
                    buf.AppendFormat("for {0} cycles ", action.cycles);
                else
                    buf.AppendFormat("for {0} times within {1} cycles ", action.count, action.cycles);
            }
        }

        public static bool EvalQExpression<T>(string compOp, T left, T right) where T : IComparable
        {
            switch (compOp)
            {
                case ">":
                    if (left.CompareTo(right) > 0)
                        return true;
                    break;
                case "<":
                    if (left.CompareTo(right) < 0)
                        return true;
                    break;
                case "=":
                    if (left.CompareTo(right) == 0)
                        return true;
                    break;
                case "!=":
                case "<>":
                    if (left.CompareTo(right) != 0)
                        return true;
                    break;
                default:
                    Logger.Log.Error("Unknown comparison operator\n");
                    return false;
            }
            return false;
        }

        #endregion

        #region Private Methods

        //TODO

        #endregion
    }
}