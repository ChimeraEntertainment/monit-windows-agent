namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServiceSystemCpu
    {

        private decimal userField;

        private decimal systemField;

        /// <remarks/>
        public decimal user
        {
            get
            {
                return this.userField;
            }
            set
            {
                this.userField = value;
            }
        }

        /// <remarks/>
        public decimal system
        {
            get
            {
                return this.systemField;
            }
            set
            {
                this.systemField = value;
            }
        }
    }
}