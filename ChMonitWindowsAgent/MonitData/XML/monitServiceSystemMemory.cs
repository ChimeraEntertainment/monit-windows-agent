namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServiceSystemMemory
    {

        private decimal percentField;

        private uint kilobyteField;

        /// <remarks/>
        public decimal percent
        {
            get
            {
                return this.percentField;
            }
            set
            {
                this.percentField = value;
            }
        }

        /// <remarks/>
        public uint kilobyte
        {
            get
            {
                return this.kilobyteField;
            }
            set
            {
                this.kilobyteField = value;
            }
        }
    }
}