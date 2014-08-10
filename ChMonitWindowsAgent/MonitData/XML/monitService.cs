namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitService
    {

        private string nameField;

        private double collected_secField;

        private uint collected_usecField;

        private int statusField;

        private int status_hintField;

        private int monitorField;

        private int monitormodeField;

        private int pendingactionField;

        private ushort modeField;

        private bool modeFieldSpecified;

        private int uidField;

        private bool uidFieldSpecified;

        private int gidField;

        private bool gidFieldSpecified;

        private uint flagsField;

        private bool flagsFieldSpecified;

        private monitServiceBlock blockField;

        private monitServiceInode inodeField;

        private monitServiceSystem systemField;

        private monitServicePort portField;

        private int typeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        public double collected_sec
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
        public int status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public int status_hint
        {
            get
            {
                return this.status_hintField;
            }
            set
            {
                this.status_hintField = value;
            }
        }

        /// <remarks/>
        public int monitor
        {
            get
            {
                return this.monitorField;
            }
            set
            {
                this.monitorField = value;
            }
        }

        /// <remarks/>
        public int monitormode
        {
            get
            {
                return this.monitormodeField;
            }
            set
            {
                this.monitormodeField = value;
            }
        }

        /// <remarks/>
        public int pendingaction
        {
            get
            {
                return this.pendingactionField;
            }
            set
            {
                this.pendingactionField = value;
            }
        }

        /// <remarks/>
        public ushort mode
        {
            get
            {
                return this.modeField;
            }
            set
            {
                this.modeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool modeSpecified
        {
            get
            {
                return this.modeFieldSpecified;
            }
            set
            {
                this.modeFieldSpecified = value;
            }
        }

        /// <remarks/>
        public int uid
        {
            get
            {
                return this.uidField;
            }
            set
            {
                this.uidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool uidSpecified
        {
            get
            {
                return this.uidFieldSpecified;
            }
            set
            {
                this.uidFieldSpecified = value;
            }
        }

        /// <remarks/>
        public int gid
        {
            get
            {
                return this.gidField;
            }
            set
            {
                this.gidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool gidSpecified
        {
            get
            {
                return this.gidFieldSpecified;
            }
            set
            {
                this.gidFieldSpecified = value;
            }
        }

        /// <remarks/>
        public uint flags
        {
            get
            {
                return this.flagsField;
            }
            set
            {
                this.flagsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool flagsSpecified
        {
            get
            {
                return this.flagsFieldSpecified;
            }
            set
            {
                this.flagsFieldSpecified = value;
            }
        }

        /// <remarks/>
        public monitServiceBlock block
        {
            get
            {
                return this.blockField;
            }
            set
            {
                this.blockField = value;
            }
        }

        /// <remarks/>
        public monitServiceInode inode
        {
            get
            {
                return this.inodeField;
            }
            set
            {
                this.inodeField = value;
            }
        }

        /// <remarks/>
        public monitServiceSystem system
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

        /// <remarks/>
        public monitServicePort port
        {
            get
            {
                return this.portField;
            }
            set
            {
                this.portField = value;
            }
        }

        /// <remarks/>
        // [System.Xml.Serialization.XmlAttributeAttribute()]
        // AN@CH No attribute, otherwise the data is not displayed correctly
        public int type
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
    }
}