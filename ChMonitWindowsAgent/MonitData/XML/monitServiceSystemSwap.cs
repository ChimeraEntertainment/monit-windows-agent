namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServiceSystemSwap
    {

        private double percentField;

        private byte kilobyteField;

        /// <remarks/>
        public double percent
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
        public byte kilobyte
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