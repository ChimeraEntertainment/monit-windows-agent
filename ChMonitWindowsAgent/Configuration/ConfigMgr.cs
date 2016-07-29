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
                    _config = LoadConfig();
                return _config;
            }
        }

        public static void ReloadConfig()
        {
            _config = null;
            _config = LoadConfig();
        }

        private static MonitWindowsAgentConfig LoadConfig()
        {
            // load the config xml, generate if it doesn´t exist
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configFileName = typeof (MonitWindowsAgentConfig).Name + ".xml";
            var configFilePathName = Path.Combine(path, configFileName);

            var ser = new XmlSerializer(typeof (MonitWindowsAgentConfig));

            MonitWindowsAgentConfig config = null;

            if (!File.Exists(configFilePathName))
            {
                return WriteDefaultConfig(configFilePathName);
            }

            using (var str = new FileStream(configFilePathName, FileMode.Open, FileAccess.Read))
            {
                config = ser.Deserialize(str) as MonitWindowsAgentConfig;
            }

            // set period to ms
            if (config.Period < 1000)
                config.Period *= 1000;

            return config;
        }

        private static MonitWindowsAgentConfig WriteDefaultConfig(string configFilePathName)
        {
            using (var str = File.CreateText(configFilePathName))
            {
                str.Write(ChMonitoring.Properties.Resources.MonitWindowsAgentConfig);
            }
            return LoadConfig();
        }
    }
}