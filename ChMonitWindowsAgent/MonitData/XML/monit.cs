using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChMonitoring.MonitData.XML
{


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "monit", Namespace = "", IsNullable = false)]
    public partial class monit
    {

        private monitServer serverField;

        private monitPlatform platformField;

        private monitService[] servicesField;

        private object servicegroupsField;

        private monitEvent eventField;

        private string idField;

        private double incarnationField;

        private string versionField;

        /// <remarks/>
        public monitServer server
        {
            get
            {
                return this.serverField;
            }
            set
            {
                this.serverField = value;
            }
        }

        /// <remarks/>
        public monitPlatform platform
        {
            get
            {
                return this.platformField;
            }
            set
            {
                this.platformField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("service", IsNullable = false)]
        // [System.Xml.Serialization.XmlElementAttribute("service")]
        public monitService[] services
        {
            get
            {
                return this.servicesField;
            }
            set
            {
                this.servicesField = value;
            }
        }

        /// <remarks/>
        public object servicegroups
        {
            get
            {
                return this.servicegroupsField;
            }
            set
            {
                this.servicegroupsField = value;
            }
        }

        /// <remarks/>
        public monitEvent @event
        {
            get
            {
                return this.eventField;
            }
            set
            {
                this.eventField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public double incarnation
        {
            get
            {
                return this.incarnationField;
            }
            set
            {
                this.incarnationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitEvent
    {

        private uint collected_secField;

        private uint collected_usecField;

        private string serviceField;

        private byte typeField;

        private uint idField;

        private byte stateField;

        private byte actionField;

        private string messageField;

        /// <remarks/>
        public uint collected_sec
        {
            get
            {
                return this.collected_secField;
            }
            set
            {
                this.collected_secField = value;
            }
        }

        /// <remarks/>
        public uint collected_usec
        {
            get
            {
                return this.collected_usecField;
            }
            set
            {
                this.collected_usecField = value;
            }
        }

        /// <remarks/>
        public string service
        {
            get
            {
                return this.serviceField;
            }
            set
            {
                this.serviceField = value;
            }
        }

        /// <remarks/>
        public byte type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        public uint id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public byte state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public byte action
        {
            get
            {
                return this.actionField;
            }
            set
            {
                this.actionField = value;
            }
        }

        /// <remarks/>
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }


}
