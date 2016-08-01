using System.Collections.Generic;
using System.Xml.Serialization;

namespace ChMonitoring.Configuration
{
    public abstract class ServiceConfig
    {
        [XmlElement] public Every Every;

        [XmlElement] public ActionRate ActionRate;

        [XmlAttribute] public int MonitoringMode;

        [XmlAttribute] public string Name;
    }

    public class ProcessConfig : ServiceConfig
    {
        [XmlArray("Resources")] [XmlArrayItem("Resource", typeof (Resource))] public List<Resource> Resources;
    }

    public class FilesystemConfig : ServiceConfig
    {
        [XmlArray("Filesystems")] [XmlArrayItem("Filesystem", typeof (Filesystem))] public List<Filesystem> Filesystems;
    }

    public class Every
    {
        [XmlAttribute] public int CycleNumber;

        [XmlAttribute] public int Type;
    }

    public class ActionRate
    {
        [XmlAttribute] public int ActionType;

        [XmlAttribute] public int Count;

        [XmlAttribute] public int Cycle;
    }

    public class MMonit
    {
        [XmlAttribute] public string Password;

        [XmlAttribute] public string Url;

        [XmlAttribute] public string Username;
    }

    public class Mailserver
    {
        [XmlAttribute] public string Host;

        [XmlAttribute] public string Password;

        [XmlAttribute] public int Port;

        [XmlAttribute] public string Username;
    }

    public class Httpd
    {
        [XmlAttribute] public string BindIp;

        [XmlAttribute] public string Password;

        [XmlAttribute] public short Port;

        [XmlAttribute] public bool SSL;

        [XmlAttribute] public string Username;
    }

    public class Filesystem
    {
        [XmlAttribute] public int ActionType;

        [XmlAttribute] public string ComparisonOperator;

        [XmlAttribute] public long LimitAbsolute;

        [XmlAttribute] public short LimitPercent;

        [XmlAttribute] public int Type;
    }

    public class Resource
    {
        [XmlAttribute] public int ActionType;

        [XmlAttribute] public string ComparisonOperator;

        [XmlAttribute] public long Limit;

        [XmlAttribute] public int Type;
    }

    /// <summary>
    ///     Config object to be serialized into XML.
    ///     TODO: Find a way to use the original monitrc controlfile in the future?
    /// </summary>
    public class MonitWindowsAgentConfig
    {
        [XmlElement] public int Period;

        [XmlElement] public int FailedStarts;

        [XmlElement] public string DisplayName;

        [XmlElement] public Httpd Httpd;

        [XmlArray("Mailservers")] [XmlArrayItem("Mailserver", typeof (Mailserver))] public List<Mailserver> Mailservers;

        [XmlArray("MMonits")] [XmlArrayItem("MMonit", typeof (MMonit))] public List<MMonit> MMonits;

        // create an empty xml 
        [XmlArray("Services")] [XmlArrayItem("Process", typeof (ProcessConfig))] [XmlArrayItem("Filesystem", typeof (FilesystemConfig))] public List<ServiceConfig> Services;
    }
}