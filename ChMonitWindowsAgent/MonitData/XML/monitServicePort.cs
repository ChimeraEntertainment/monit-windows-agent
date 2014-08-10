namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServicePort
    {

        private string hostnameField;

        private ushort portnumberField;

        private string requestField;

        private string protocolField;

        private string typeField;

        private decimal responsetimeField;

        /// <remarks/>
        public string hostname
        {
            get
            {
                return this.hostnameField;
            }
            set
            {
                this.hostnameField = value;
            }
        }

        /// <remarks/>
        public ushort portnumber
        {
            get
            {
                return this.portnumberField;
            }
            set
            {
                this.portnumberField = value;
            }
        }

        /// <remarks/>
        public string request
        {
            get
            {
                return this.requestField;
            }
            set
            {
                this.requestField = value;
            }
        }

        /// <remarks/>
        public string protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <remarks/>
        public string type
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
        public decimal responsetime
        {
            get
            {
                return this.responsetimeField;
            }
            set
            {
                this.responsetimeField = value;
            }
        }
    }
}