using System;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.IO;
using ChMonitoring.Helpers;
using ChMonitoring.MonitData;
using ChMonitoring.MonitLogic;
using ChMonitoring.Monitoring;
using ChMonitoring.MonitData.XML;

namespace ChMonitoring
{
    /**
     * TODOS:
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
     * - Implement event alerting..
     *  - in <monit> root node:
     *      	<event>
		            <collected_sec>1407169749</collected_sec>
		            <collected_usec>882604</collected_usec>
		            <service>Monit</service>
		            <type>5</type>
		            <id>65536</id> <!-- Event_Instance   = 0x10000, --> 
		            <state>2</state>
		            <action>6</action>
		            <message><![CDATA[Monit reloaded]]></message>
	            </event>
     * - In favor of testability etc, get rid of static methods and instances
     * **/
    class MonitWindowsAgent
    {
        private Timer m_timer;
        private EventLog m_log;

        private string m_configFileName;
        private string m_configFilePathName;
        // private XmlDocument m_configXmlDoc;

        private MMonitClient m_mMonitClient;
        private ServiceMonitoring m_serviceMonitoring;
        private MonitServer m_monitServer;

        public static MonitWindowsAgent Instance { private set; get; }

        public MonitWindowsAgent(EventLog log)
        {
            Instance = this;

            // save the service log
            m_log = log;

            // LoadConfig();
            InitConfig();
            Logger.Init();
            InitTimer();

            m_serviceMonitoring = new ServiceMonitoring();
            m_serviceMonitoring.Start(log);

            m_mMonitClient = new MMonitClient();
            m_mMonitClient.Start();

            m_monitServer = new MonitServer();
            m_monitServer.Start();
        }

        private void InitTimer()
        {
            m_timer = new Timer(ConfigMgr.Config.Period);
            m_timer.Elapsed += DoPeriodicCheck;
            m_timer.AutoReset = true;
        }

        private void DoPeriodicCheck(object sender, ElapsedEventArgs e)
        {
            m_serviceMonitoring.CheckServicesRunning();
            m_mMonitClient.Push();
        }

        private void InitConfig()
        {
           // m_configXmlDoc = new XmlDocument();
            
            // load the config xml, generate if it doesn´t exist
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            m_configFileName = typeof(MonitWindowsAgentConfig).Name + ".xml";
            m_configFilePathName = Path.Combine(path, m_configFileName);

            ConfigMgr.LoadConfig(m_configFilePathName);

        }

        public void Start()
        {
            // start the timer
            m_timer.Start();

            DoPeriodicCheck(m_timer, null);
        }


        public SerializedInfo GetStatusXmlData()
        {
            var data = new monit();
            data.id = UniqueWindowsId.GetOrCreateUniqueId();
            data.incarnation = SystemStats.ProcessRunningInSec();
            data.server = MonitStatusGenericData.GetMonitServerData();
            data.platform = MonitStatusGenericData.GetMonitPlatformData();
            data.version = "5.6"; // TODO, decide how to use the correct version number here in the future

            var services = new List<monitService>();
            services.AddRange(GetFilesystemServices()); // TODO, should depend on config / controlfile
            services.Add(GetSystemService()); // TODO, should depend on config / controlfile
            // data.service.Add(GetHostService()); // TODO, depends on config / controlfile..

            data.services = services.ToArray();

            var status = SerializeMgr.Serialize(data);
            return status;
        }

        private monitService GetHostService()
        {
            // TODO
            return null;
        }

        private monitService GetSystemService()
        {
            var systemService = GetNewServiceSkeleton(MonitServiceType.TYPE_SYSTEM);
            systemService.system = new monitServiceSystem();

            systemService.name = SystemStats.GetHostname();
            
            // CPU
            systemService.system.cpu = new monitServiceSystemCpu
            {
                system = SystemStats.GetCPULoadPercentage(),
                user = 0,
            };

            // MEMORY
            var totalMem = PerformanceInfo.GetTotalMemoryInKiB();
            systemService.system.memory = new monitServiceSystemMemory()
            {
                kilobyte = totalMem,
                // calculating free mem, then substracting this from 100 to get used mem percentage
                percent = Math.Round(100 - ((decimal)PerformanceInfo.GetPhysicalAvailableMemoryInKiB() / (decimal)totalMem) * 100, 1),
            };

            // LOAD
            systemService.system.load = new monitServiceSystemLoad()
            {
                avg01 = 0.07,
                avg05 = 0.03,
                avg15 = 0.08,
            };

            // SWAP
            // Possible to use this on windows?
            systemService.system.swap = new monitServiceSystemSwap()
            {
                kilobyte = 0,
                percent = 0,
            };

            return systemService;
        }

        private List<monitService> GetFilesystemServices()
        {
            var fileSystemServices = new List<monitService>();

            // ADD ALL DRIVES AUTOMATICALLY
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                {
                    var fsService = GetNewServiceSkeleton(MonitServiceType.TYPE_FILESYSTEM);
                    fsService.name = drive.VolumeLabel + " (" + drive.DriveFormat.ToUpper() +  ")";

                    fsService.block = new monitServiceBlock()
                    {
                        total = drive.TotalFreeSpace,
                        usage = drive.TotalFreeSpace - drive.AvailableFreeSpace,
                        percent = (drive.TotalFreeSpace == 0) ? 100 : ((drive.AvailableFreeSpace/drive.TotalFreeSpace)*100),
                    };

                    fileSystemServices.Add(fsService);
                }
            }

            return fileSystemServices;
        }

        /// <summary>
        /// Get "frame"-xml-data which is the same for different usecases
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private monitService GetNewServiceSkeleton(MonitServiceType type)
        {
            var serviceSekeleton = new monitService();
            serviceSekeleton.name = SystemStats.GetHostname();

            serviceSekeleton.type = (int)type;
            serviceSekeleton.status = 0; // "error". Error flags bitmap, See MonitEventTable
            serviceSekeleton.status_hint = 0; // "error_hint" See MonitEventTable
            serviceSekeleton.monitor = (int)MonitMonitorStateType.MONITOR_YES;
            serviceSekeleton.monitormode = (int)MonitMonitorModeType.MODE_ACTIVE; // default

            var tf = Timing.GetTimingFraction();
            serviceSekeleton.collected_sec = tf.Timestamp;
            serviceSekeleton.collected_usec = tf.Usec;

            return serviceSekeleton;
        }
    }
}
