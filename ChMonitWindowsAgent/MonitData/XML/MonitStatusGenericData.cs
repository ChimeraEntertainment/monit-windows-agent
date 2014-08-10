using System;
using ChMonitoring.Helpers;

namespace ChMonitoring.MonitData.XML
{
    /// <summary>
    /// Supplies skeleton data which is needed more often for xml generation
    /// </summary>
    class MonitStatusGenericData
    {
        // TODO object caching where possible...
        // TODO read all from config file... (monitrc)
        public static monitServer GetMonitServerData()
        {
            var data = new monitServer();

            data.controlfile = "none"; // TODO
            data.startdelay = 0; // TODO

            data.localhostname = SystemStats.GetHostname();

            data.httpd = new monitServerHttpd()
            {
                address = ConfigMgr.Config.HttpdBindIp,
                port = ConfigMgr.Config.HttpdPort,
                ssl = (byte)(ConfigMgr.Config.HttpdSSL ? 1 : 0), // TODO
            };

            data.credentials = new monitServerCredentials()
            {
                password = ConfigMgr.Config.HttpdPassword,
                username = ConfigMgr.Config.HttpdUsername
            };

            data.poll = 120;  // TODO

            return data;
        }

        public static monitPlatform GetMonitPlatformData()
        {
            var data = new monitPlatform();

            data = new monitPlatform();
            data.name = Environment.OSVersion.Platform.ToString();
            data.version = Environment.OSVersion.Version.Build.ToString();
            data.machine = CPUAchitecture.IsOS64Bit() ? "x86_64" : "x86";
            data.release = Environment.OSVersion.Version.ToString(); // Environment.OSVersion.Version.Build.ToString();
            data.cpu = SystemStats.GetCPUCoreCount();
            data.memory = SystemStats.GetMemorySizeKB();

            // TODO
            data.swap = 0;

            return data;
        }
    }
}
