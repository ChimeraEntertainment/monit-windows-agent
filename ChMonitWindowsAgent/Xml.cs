using System.Text;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;
using ChMonitoring.Properties;

namespace ChMonitoring
{
    /// <summary>
    ///     The XML-Helper Class
    /// </summary>
    internal class Xml
    {
        #region Public Methods

        public static string StatusXml(Event_T E, MonitLevelType L, int V, string myip)
        {
            var centerString = new StringBuilder();
            if (V == 2)
                centerString.Append("<services>");
            foreach (var S in MonitWindowsAgent.servicelist_conf)
            {
                statusService(S, centerString, L, V);
            }
            if (V == 2)
            {
                centerString.Append("</services><servicegroups>");
                foreach (var SG in MonitWindowsAgent.servicegrouplist)
                {
                    var serviceGroupXmlBuilder = new OutputBuilder(Resources.ServiceGroupXml);
                    var serviceGroupString = new StringBuilder();
                    foreach (var sgm in SG.members)
                    {
                        serviceGroupString.AppendFormat("<service>{0}</service>", sgm.name);
                    }
                    serviceGroupXmlBuilder.ReplaceAllTags(SG.name, serviceGroupString.ToString());
                    centerString.Append(serviceGroupXmlBuilder.GetResult());
                }
                centerString.Append("</servicegroups>");
            }
            if (E != null)
            {
                var eventXmlBuilder = new OutputBuilder(Resources.EventXml);
                var timeval = E.collected;
                var source = Event.GetSource(E);
                eventXmlBuilder.ReplaceAllTags(Timing.GetTimestamp(timeval), Timing.GetUsec(timeval),
                    E.id == MonitEventType.Event_Instance ? "Monit" : E.source, (int) E.type, (int) E.id, (int) E.state,
                    (int) Event.GetAction(E), _escapeCDATA(E.message),
                    (source != null && !string.IsNullOrEmpty(source.token))
                        ? string.Format("<token>{0}</token>", source.token)
                        : "");
                centerString.Append(eventXmlBuilder.GetResult());
            }

            var headerFooterXmlBuilder = new OutputBuilder(Resources.HeaderFooterXml);
            var run = MonitWindowsAgent.Run;
            var systemInfo = MonitWindowsAgent.systeminfo;
            var headString = new StringBuilder();
            if (V == 2)
                headString.AppendFormat("<monit id=\"{0}\" incarnation=\"{1}\" version=\"{2}\"><server>", run.id,
                    Timing.GetTimestamp(run.incarnation), MonitWindowsAgent.VERSION);
            else
                headString.AppendFormat(
                    "<monit><server><id>{0}</id><incarnation>{1}</incarnation><version>{2}</version>", run.id,
                    Timing.GetTimestamp(run.incarnation), MonitWindowsAgent.VERSION);

            var httpdBuilder = new StringBuilder();
            if (MonitWindowsAgent.Run.httpd != null)
            {
                var httpd = MonitWindowsAgent.Run.httpd;
                httpdBuilder.AppendFormat("<httpd><address>{0}</address><port>{1}</port><ssl>{2}</ssl></httpd>",
                    httpd.address, httpd.port, httpd.ssl ? "1" : "0");
                if (httpd.credentials != null && httpd.credentials.Count > 0)
                    httpdBuilder.AppendFormat(
                        "<credentials><username>{0}</username><password>{1}</password></credentials>",
                        httpd.credentials[0].uname, httpd.credentials[0].passwd);
            }

            headerFooterXmlBuilder.ReplaceAllTags(headString.ToString(), "5", run.polltime, run.startdelay,
                run.system[0].name, run.controlfile, httpdBuilder.ToString(), systemInfo.uname.sysname,
                systemInfo.uname.release, systemInfo.uname.version, systemInfo.uname.machine, systemInfo.cpus,
                systemInfo.mem_kbyte_max, systemInfo.swap_kbyte_max, centerString.ToString());
            return headerFooterXmlBuilder.GetResult();
        }

        #endregion

        #region Private Methods

        private static void statusService(Service_T S, StringBuilder B, MonitLevelType L, int V)
        {
            var serviceXmlBuilder = new OutputBuilder(Resources.ServiceXml);

            var timeval = S.collected;
            var headString = new StringBuilder();
            if (V == 2)
                headString.AppendFormat("<service name=\"{0}\"><type>{1}</type>", S.name, (int) S.type);
            else
                headString.AppendFormat("<service type=\"{0}\"><name>{1}</name>", (int) S.type, S.name);

            var everyString = new StringBuilder();
            if (S.every.type != MonitEveryType.Every_Cycle)
            {
                everyString.AppendFormat("<every><type>{0}</type>", (int) S.every.type);
                if (S.every.type == MonitEveryType.Every_SkipCycles)
                {
                    var cycle = S.every as CycleEvery_T;
                    everyString.AppendFormat("<counter>{0}</counter><number>{1}</number>", cycle.counter, cycle.number);
                }
                else
                {
                    var cron = S.every as CronEvery_T;
                    everyString.AppendFormat("<cron>{0}</cron>", cron.cron);
                }
                everyString.Append("</every>");
            }

            var centerString = new StringBuilder();
            if (L == MonitLevelType.Level_Full)
            {
                if (Util.HasServiceStatus(S))
                {
                    switch (S.type)
                    {
                        case MonitServiceType.Service_File:
                            var file = S.inf as FileInfo_T;
                            //centerString.AppendFormat(@"<mode>%o</mode>"
                            //        "<uid>%d</uid>"
                            //        "<gid>%d</gid>"
                            //        "<timestamp>%lld</timestamp>"
                            //        "<size>%llu</size>",
                            //        file.mode & 07777,
                            //        (int)file.uid,
                            //        (int)file.gid,
                            //        (long long)file.timestamp,
                            //        (unsigned long long)file.size);
                            //if (S.checksum)
                            //        centerString.AppendFormat(B, "<checksum type=\"%s\">%s</checksum>", checksumnames[S.checksum.type], file.cs_sum);
                            break;

                        case MonitServiceType.Service_Directory:
                            var directory = S.inf as DirectoryInfo_T;
                            //centerString.AppendFormat(@"<mode>%o</mode>"
                            //        "<uid>%d</uid>"
                            //        "<gid>%d</gid>"
                            //        "<timestamp>%lld</timestamp>",
                            //        directory.mode & 07777,
                            //        (int)directory.uid,
                            //        (int)directory.gid,
                            //        (long long)directory.timestamp);
                            break;

                        case MonitServiceType.Service_Fifo:
                            var fifo = S.inf as FiFoInfo_T;
                            //centerString.AppendFormat(@"<mode>%o</mode>"
                            //        "<uid>%d</uid>"
                            //        "<gid>%d</gid>"
                            //        "<timestamp>%lld</timestamp>",
                            //        fifo.mode & 07777,
                            //        (int)fifo.uid,
                            //        (int)fifo.gid,
                            //        (long long)fifo.timestamp);
                            break;

                        case MonitServiceType.Service_Filesystem:
                            var filesystem = S.inf as FileSystemInfo_T;
                            centerString.AppendFormat(
                                "<mode>{0}</mode><uid>{1}</uid><gid>{2}</gid><flags>{3}</flags><block><percent>{4}</percent><usage>{5}</usage><total>{6}</total></block>",
                                filesystem.mode,
                                filesystem.uid,
                                filesystem.gid,
                                filesystem.flags,
                                filesystem.space_percent/10f,
                                filesystem.f_bsize > 0 ? filesystem.space_total/1048576.0*filesystem.f_bsize : 0,
                                filesystem.f_bsize > 0 ? filesystem.f_blocks/1048576.0*filesystem.f_bsize : 0);
                            if (filesystem.f_files > 0)
                            {
                                centerString.AppendFormat(
                                    "<inode><percent>{0}</percent><usage>{1}</usage><total>{2}</total></inode>",
                                    filesystem.inode_percent/10f,
                                    filesystem.inode_total,
                                    filesystem.f_files);
                            }
                            break;

                        case MonitServiceType.Service_Net:
                            var net = S.inf as NetInfo_T;
                            //centerString.AppendFormat(@"<link>"
                            //        "<state>%d</state>"
                            //        "<speed>%lld</speed>"
                            //        "<duplex>%d</duplex>"
                            //        "<download>"
                            //        "<packets>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</packets>"
                            //        "<bytes>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</bytes>"
                            //        "<errors>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</errors>"
                            //        "</download>"
                            //        "<upload>"
                            //        "<packets>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</packets>"
                            //        "<bytes>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</bytes>"
                            //        "<errors>"
                            //        "<now>%lld</now>"
                            //        "<total>%lld</total>"
                            //        "</errors>"
                            //        "</upload>"
                            //        "</link>",
                            //        Link_getState(net.stats),
                            //        Link_getSpeed(net.stats),
                            //        Link_getDuplex(net.stats),
                            //        Link_getPacketsInPerSecond(net.stats),
                            //        Link_getPacketsInTotal(net.stats),
                            //        Link_getBytesInPerSecond(net.stats),
                            //        Link_getBytesInTotal(net.stats),
                            //        Link_getErrorsInPerSecond(net.stats),
                            //        Link_getErrorsInTotal(net.stats),
                            //        Link_getPacketsOutPerSecond(net.stats),
                            //        Link_getPacketsOutTotal(net.stats),
                            //        Link_getBytesOutPerSecond(net.stats),
                            //        Link_getBytesOutTotal(net.stats),
                            //        Link_getErrorsOutPerSecond(net.stats),
                            //        Link_getErrorsOutTotal(net.stats));
                            break;
                        case MonitServiceType.Service_Process:
                            var process = S.inf as ProcessInfo_T;
                            centerString.AppendFormat(
                                "<pid>{0}</pid><ppid>{1}</ppid><uid>{2}</uid><euid>{3}</euid><gid>{4}</gid><uptime>{5}</uptime>",
                                process.pid,
                                process.ppid,
                                process.uid,
                                process.euid,
                                process.gid,
                                process.uptime);
                            if (MonitWindowsAgent.Run.doprocess)
                            {
                                centerString.AppendFormat(
                                    "<children>{0}</children><memory><percent>{1}</percent><percenttotal>{2}</percenttotal><kilobyte>{3}</kilobyte><kilobytetotal>{4}</kilobytetotal></memory><cpu><percent>{5}</percent><percenttotal>{6}</percenttotal></cpu>",
                                    process.children,
                                    process.mem_percent/10f,
                                    process.total_mem_percent/10f,
                                    process.mem_kbyte,
                                    process.total_mem_kbyte,
                                    process.cpu_percent/10f,
                                    process.total_cpu_percent/10f);
                            }
                            break;

                        default:
                            break;
                    }
                    //for (Icmp_T i = S.icmplist; i; i = i.next) {
                    //        centerString.AppendFormat(@"<icmp>"
                    //                            "<type>%s</type>"
                    //                            "<responsetime>%.3f</responsetime>"
                    //                            "</icmp>",
                    //                            icmpnames[i.type],
                    //                            i.is_available ? i.response : -1.);
                    //}
                    //for (Port_T p = S.portlist; p; p = p.next) {
                    //        centerString.AppendFormat(@"<port>"
                    //                            "<hostname>%s</hostname>"
                    //                            "<portnumber>%d</portnumber>"
                    //                            "<request><![CDATA[%s]]></request>"
                    //                            "<protocol>%s</protocol>"
                    //                            "<type>%s</type>"
                    //                            "<responsetime>%.3f</responsetime>"
                    //                            "</port>",
                    //                            p.hostname ? p.hostname : "",
                    //                            p.port,
                    //                            p.request ? p.request : "",
                    //                            p.protocol.name ? p.protocol.name : "",
                    //                            Util_portTypeDescription(p),
                    //                            p.is_available ? p.response : -1.);
                    //}
                    //for (Port_T p = S.socketlist; p; p = p.next) {
                    //        centerString.AppendFormat(@"<unix>"
                    //                            "<path>%s</path>"
                    //                            "<protocol>%s</protocol>"
                    //                            "<responsetime>%.3f</responsetime>"
                    //                            "</unix>",
                    //                            p.pathname ? p.pathname : "",
                    //                            p.protocol.name ? p.protocol.name : "",
                    //                            p.is_available ? p.response : -1.);
                    //}
                    if (S.type == MonitServiceType.Service_System && MonitWindowsAgent.Run.doprocess)
                    {
                        centerString.AppendFormat(
                            "<system><load><avg01>{0}</avg01><avg05>{1}</avg05><avg15>{2}</avg15></load><cpu><user>{3}</user><system>{4}</system></cpu><memory><percent>{5}</percent><kilobyte>{6}</kilobyte></memory><swap><percent>{7}</percent><kilobyte>{8}</kilobyte></swap></system>",
                            MonitWindowsAgent.systeminfo.loadavg[0],
                            MonitWindowsAgent.systeminfo.loadavg[1],
                            MonitWindowsAgent.systeminfo.loadavg[2],
                            MonitWindowsAgent.systeminfo.total_cpu_user_percent > 0
                                ? MonitWindowsAgent.systeminfo.total_cpu_user_percent/10f
                                : 0,
                            MonitWindowsAgent.systeminfo.total_cpu_syst_percent > 0
                                ? MonitWindowsAgent.systeminfo.total_cpu_syst_percent/10f
                                : 0,
                            MonitWindowsAgent.systeminfo.total_mem_percent/10f,
                            MonitWindowsAgent.systeminfo.total_mem_kbyte,
                            MonitWindowsAgent.systeminfo.total_swap_percent/10f,
                            MonitWindowsAgent.systeminfo.total_swap_kbyte);
                    }
                    if (S.type == MonitServiceType.Service_Program && S.program.started != null)
                    {
                        centerString.AppendFormat(
                            "<program><started>{0}</started><status>{1}</status><output><![CDATA[",
                            Timing.GetTimestamp(S.program.started),
                            S.program.exitStatus);
                        centerString.Append(_escapeCDATA(S.program.output.ToString()));
                        centerString.Append("]]></output></program>");
                    }
                }
            }

            serviceXmlBuilder.ReplaceAllTags(headString.ToString(), Timing.GetTimestamp(timeval),
                Timing.GetUsec(timeval), S.error, S.error_hint, (int) S.monitor, (int) S.mode, (int) S.doaction,
                everyString.ToString(), centerString.ToString());
            B.Append(serviceXmlBuilder.GetResult());
        }

        private static string _escapeCDATA(string buf)
        {
            var result = new StringBuilder(buf.Length);
            for (var i = 0; i < buf.Length; i++)
            {
                if (buf[i] == '>' && i > 1 && (buf[i - 1] == ']' && buf[i - 2] == ']'))
                    result.Append("&gt;");
                else
                    result.Append(buf[i]);
            }
            return result.ToString();
        }

        #endregion
    }
}