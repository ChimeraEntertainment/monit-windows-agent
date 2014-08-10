namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServiceSystem
    {

        private monitServiceSystemLoad loadField;

        private monitServiceSystemCpu cpuField;

        private monitServiceSystemMemory memoryField;

        private monitServiceSystemSwap swapField;

        /// <remarks/>
        public monitServiceSystemLoad load
        {
            get
            {
                return this.loadField;
            }
            set
            {
                this.loadField = value;
            }
        }

        /// <remarks/>
        public monitServiceSystemCpu cpu
        {
            get
            {
                return this.cpuField;
            }
            set
            {
                this.cpuField = value;
            }
        }

        /// <remarks/>
        public monitServiceSystemMemory memory
        {
            get
            {
                return this.memoryField;
            }
            set
            {
                this.memoryField = value;
            }
        }

        /// <remarks/>
        public monitServiceSystemSwap swap
        {
            get
            {
                return this.swapField;
            }
            set
            {
                this.swapField = value;
            }
        }
    }
}