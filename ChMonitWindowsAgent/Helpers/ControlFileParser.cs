using ChMonitoring.Configuration;
using ChMonitoring.MonitData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChMonitoring.Helpers
{
    class ControlFileParser
    {
        public static void parse()
        {
            ConfigMgr.ReloadConfig();
            Run_T Run = new Run_T();
            Run.id = UniqueWindowsId.GetOrCreateUniqueId();
            Run.incarnation = SystemStats.ProcessRunningInSec();
            Run.controlfile = "none"; //TODO
            Run.startdelay = 0; //TODO
            Run.polltime = 120; //TODO
            Run.Env = new myenvironment();
            Run.Env.user = SystemStats.GetHostname();
            Run.httpd = new Httpd_T();
            Run.httpd.port = ConfigMgr.Config.Httpd.Port;
            Run.httpd.ssl = ConfigMgr.Config.Httpd.SSL;
            Run.httpd.address = ConfigMgr.Config.Httpd.BindIp;
            Run.httpd.credentials = new List<Auth_T>();
            Run.httpd.credentials.Add(new Auth_T
            {
                uname = ConfigMgr.Config.Httpd.Username,
                passwd = ConfigMgr.Config.Httpd.Password
            });

            Run.mmonits = new List<Mmonit_T>();
            Run.mmonits.Add(new Mmonit_T
            {
                url = new URL_T
                {
                    url = ConfigMgr.Config.MMonits[0].Url,
                    //port = ConfigMgr.Config.MMonits[0].Port,
                    password = ConfigMgr.Config.MMonits[0].Password,
                    user = ConfigMgr.Config.MMonits[0].Username
                }
            });

            MonitWindowsAgent.servicelist = new List<Service_T>();
            MonitWindowsAgent.servicelist_conf = new List<Service_T>();
            foreach(var sc in ConfigMgr.Config.Services)
            {
                var newS = ServiceHelper.CreateService(sc, sc.Name.ToLower());

                if (newS == null)
                    Logger.Log.Error("Service could not be created!");
                else
                {
                    if (sc is ProcessConfig)
                        ProcessHelper.AddProcess(newS);
                    else if (sc is FilesystemConfig)
                        FilesystemHelper.AddFilesystem(newS);
                }
            }

            MonitWindowsAgent.servicegrouplist = new List<ServiceGroup_T>();
            MonitWindowsAgent.systeminfo = SystemInfoHelper.GetSystemInfo();

            var service = ServiceHelper.GetSystemService();
            Run.system = new List<Service_T>();
            Run.system.Add(service);

            MonitWindowsAgent.Run = Run;
        }
    }
}
