using System.Collections.Generic;
using System.Threading;
using System.Timers;
using ChMonitoring.Configuration;
using ChMonitoring.Helpers;
using ChMonitoring.Http;
using ChMonitoring.MonitData;
using Timer = System.Timers.Timer;

namespace ChMonitoring
{
    /**
     * TODOS:
     *  - Cleanup all TODO-Commented Sections
     *  - Add Comments
     *  - Add Logging
     *  - Refactor to pattern
     *  - Implement a monitrc - reader to fill config
     *      - Try to reuse yacc settings? -> p.y ? 
     *          --> http://gppg.codeplex.com/
     *          --> http://gplex.codeplex.com/
     *          --> http://simplescript.codeplex.com/
     *  
     * - Alternatively: Create a completely new config format (xml?) for the windows agent.
     *   - Question: Is Mmonit using the configfile (as it is contained in the controlfile parameter in the status xml?)
     *   
     * - In favor of testability etc, get rid of static methods and instances
     * **/

    internal class MonitWindowsAgent
    {
        public const string SERVER_VERSION = "5.6";
        public const string VERSION = "5.12.2";
        //Global Variables
        //TODO
        public static Run_T Run;
        public static List<Service_T> servicelist;
        public static List<Service_T> servicelist_conf;
        public static List<ServiceGroup_T> servicegrouplist;
        public static SystemInfo_T systeminfo;
        public static Collector m_mMonitClient;

        public static string[] actionnames =
        {
            "ignore", "alert", "restart", "stop", "exec", "unmonitor", "start",
            "monitor", ""
        };

        public static string[] modenames = {"active", "passive", "manual"};
        public static string[] checksumnames = {"UNKNOWN", "MD5", "SHA1"};

        public static Dictionary<string, string> operatornames = new Dictionary<string, string>
        {
            {">", "greater than"},
            {"<", "less than"},
            {"=", "equal to"},
            {"!=", "not equal to"},
            {"<>", "changed"}
        };

        public static string[] statusnames =
        {
            "Accessible", "Accessible", "Accessible", "Running",
            "Online with all services", "Running", "Accessible", "Status ok", "UP"
        };

        public static string[] servicetypes =
        {
            "Filesystem", "Directory", "File", "Process", "Remote Host", "System",
            "Fifo", "Program", "Network"
        };

        public static string[] pathnames = {"Path", "Path", "Path", "Pid file", "Path", "", "Path"};

        public static string[] icmpnames =
        {
            "Reply", "", "", "Destination Unreachable", "Source Quench", "Redirect", "",
            "", "Ping", "", "", "Time Exceeded", "Parameter Problem", "Timestamp Request", "Timestamp Reply",
            "Information Request", "Information Reply", "Address Mask Request", "Address Mask Reply"
        };

        public static string[] sslnames = {"auto", "v2", "v3", "tlsv1", "tlsv1.1", "tlsv1.2", "none"};
        private readonly Timer m_timer;
        private bool heartbeatRunning;

        public MonitWindowsAgent()
        {
            Run = new Run_T();
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

            servicelist = new List<Service_T>();
            servicelist_conf = new List<Service_T>();
            ConfigMgr.Config.Services.ForEach(sc =>
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
            });

            servicegrouplist = new List<ServiceGroup_T>();
            systeminfo = SystemInfoHelper.GetSystemInfo();

            m_timer = new Timer(ConfigMgr.Config.Period);
            m_timer.Elapsed += DoPeriodicCheck;
            m_timer.AutoReset = true;

            m_mMonitClient = new Collector();

            var service = ServiceHelper.GetSystemService();
            Run.system = new List<Service_T>();
            Run.system.Add(service);
        }

        private void DoPeriodicCheck(object sender, ElapsedEventArgs e)
        {
            heartbeatRunning = true;
            Collector.HandleMmonit(null);
            heartbeatRunning = false;
        }

        public void Start()
        {
            Server.Start();

            Event.Post(Run.system[0], MonitEventType.Event_Instance, MonitStateType.State_Changed,
                Run.system[0].action_MONIT_START, "Monit started");

            // start the timer
            m_timer.Start();

            DoPeriodicCheck(m_timer, null);

            Update();
        }

        private void Update()
        {
            while (true)
            {
                if (!heartbeatRunning)
                    Validate.validate();

                if (!Run.doaction)
                    Thread.Sleep(Run.polltime);

                //if(Run.stopped)
                //    do_exit();
                //else if(Run.doreload)
                //    do_reinit();
            }
        }

        internal void Shutdown()
        {
            Stop();
        }

        internal void Pause()
        {
            Stop();
        }

        internal void Stop()
        {
            Server.Stop();
        }
    }
}