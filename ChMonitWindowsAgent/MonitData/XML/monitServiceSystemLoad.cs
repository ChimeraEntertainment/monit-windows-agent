namespace ChMonitoring.MonitData.XML
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class monitServiceSystemLoad
    {

        private double avg01Field;

        private double avg05Field;

        private double avg15Field;

        /// <remarks/>
        public double avg01
        {
            get
            {
                return this.avg01Field;
            }
            set
            {
                this.avg01Field = value;
            }
        }

        /// <remarks/>
        public double avg05
        {
            get
            {
                return this.avg05Field;
            }
            set
            {
                this.avg05Field = value;
            }
        }

        /// <remarks/>
        public double avg15
        {
            get
            {
                return this.avg15Field;
            }
            set
            {
                this.avg15Field = value;
            }
        }
    }
}