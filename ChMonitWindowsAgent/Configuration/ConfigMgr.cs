using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ChMonitoring.Configuration
{
    internal class ConfigMgr
    {
        private static MonitWindowsAgentConfig _config;

        public static MonitWindowsAgentConfig Config
        {
            get
            {
                if (_config == null)
                    LoadConfig();
                return _config;
            }
        }

        private static void LoadConfig()
        {
            // load the config xml, generate if it doesn´t exist
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configFileName = typeof (MonitWindowsAgentConfig).Name + ".xml";
            var configFilePathName = Path.Combine(path, configFileName);

            var ser = new XmlSerializer(typeof (MonitWindowsAgentConfig));

            if (!File.Exists(configFilePathName))
            {
                _config = WriteDefaultConfig(configFilePathName);
                return;
            }

            using (var str = new FileStream(configFilePathName, FileMode.Open, FileAccess.Read))
            {
                _config = ser.Deserialize(str) as MonitWindowsAgentConfig;
            }

            // set period to ms
            if (_config.Period < 1000)
                _config.Period *= 1000;
        }

        private static MonitWindowsAgentConfig WriteDefaultConfig(string configFilePathName)
        {
            var conf = new MonitWindowsAgentConfig();
            var ser = new XmlSerializer(typeof (MonitWindowsAgentConfig));

            using (var str = new FileStream(configFilePathName, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                // standard value 30 sec
                conf.Period = 30;
                conf.MMonits = new List<MMonit>();
                conf.MMonits.Add(new MMonit
                {
                    Url = "http://localhost:8080/collector",
                    Password = "monit",
                    Username = "monit"
                });
                conf.FailedStarts = 5;

                conf.Httpd = new Httpd
                {
                    Port = 2812,
                    BindIp = "127.0.0.1",
                    Password = "monit",
                    Username = "admin"
                };

                conf.Services = new List<ServiceConfig>();
                conf.Services.Add(new ProcessConfig
                {
                    Name = "YOUR_SERVICENAME_HERE",
                    MonitoringMode = 0,
                    Resources = new List<Resource>
                    {
                        new Resource {ActionType = 1, ComparisonOperator = "<", Limit = 90, Type = 1}
                    }
                });
                ser.Serialize(str, conf);
            }

            return conf;
        }
    }
}