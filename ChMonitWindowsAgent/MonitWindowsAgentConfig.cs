using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ChMonitoring
{
    public class Service
    {
        [XmlAttribute]
        public string ServiceName;

        [XmlElement]
        public List<string> DependentServiceNames;
    }

    /// <summary>
    /// Config object to be serialized into XML.
    /// TODO: Find a way to use the original monitrc controlfile in the future?
    /// </summary>
    public class MonitWindowsAgentConfig
    {
        // create an empty xml 
        [XmlElement]
        public int Period;

        [XmlElement]
        public List<Service> Services;

        [XmlElement]
        public int FailedStarts;

        /// <summary>
        /// Without credentials
        /// </summary>
        [XmlElement]
        public string MMonitCollectorUrl;

        [XmlElement]
        public string MMonitCollectorUsername;

        [XmlElement]
        public string MMonitCollectorPassword;

        [XmlElement]
        public short HttpdPort;

        [XmlElement]
        public string HttpdUsername;

        [XmlElement]
        public string HttpdPassword;

        [XmlElement]
        public string HttpdBindIp;

        [XmlElement]
        public bool HttpdSSL;
    }
}
