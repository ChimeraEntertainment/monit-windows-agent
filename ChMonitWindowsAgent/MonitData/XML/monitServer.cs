namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServer
    {

        private string idField;

        private uint incarnationField;

        private string versionField;

        private ushort uptimeField;

        private byte pollField;

        private byte startdelayField;

        private string localhostnameField;

        private string controlfileField;

        private monitServerHttpd httpdField;

        private monitServerCredentials credentialsField;

        /// <remarks/>
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
        public uint incarnation
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

        /// <remarks/>
        public ushort uptime
        {
            get
            {
                return this.uptimeField;
            }
            set
            {
                this.uptimeField = value;
            }
        }

        /// <remarks/>
        public byte poll
        {
            get
            {
                return this.pollField;
            }
            set
            {
                this.pollField = value;
            }
        }

        /// <remarks/>
        public byte startdelay
        {
            get
            {
                return this.startdelayField;
            }
            set
            {
                this.startdelayField = value;
            }
        }

        /// <remarks/>
        public string localhostname
        {
            get
            {
                return this.localhostnameField;
            }
            set
            {
                this.localhostnameField = value;
            }
        }

        /// <remarks/>
        public string controlfile
        {
            get
            {
                return this.controlfileField;
            }
            set
            {
                this.controlfileField = value;
            }
        }

        /// <remarks/>
        public monitServerHttpd httpd
        {
            get
            {
                return this.httpdField;
            }
            set
            {
                this.httpdField = value;
            }
        }

        /// <remarks/>
        public monitServerCredentials credentials
        {
            get
            {
                return this.credentialsField;
            }
            set
            {
                this.credentialsField = value;
            }
        }
    }
}