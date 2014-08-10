namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitPlatform
    {

        private string nameField;

        private string releaseField;

        private string versionField;

        private string machineField;

        private byte cpuField;

        private uint memoryField;

        private uint swapField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string release
        {
            get
            {
                return this.releaseField;
            }
            set
            {
                this.releaseField = value;
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
        public string machine
        {
            get
            {
                return this.machineField;
            }
            set
            {
                this.machineField = value;
            }
        }

        /// <remarks/>
        public byte cpu
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
        public uint memory
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
        public uint swap
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