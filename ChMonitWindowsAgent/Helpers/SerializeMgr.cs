using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ChMonitoring.Helpers
{
    class SerializedInfo
    {
        public string SerializedString { get; set; }
        public byte[] SerializedBytes { get; set; }
    }

    class SerializeMgr
    {
        private static string m_encoding = "ISO-8859-1";

        public string Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }

        }

        public static SerializedInfo Serialize<T>(T obj)
        {
            var info = new SerializedInfo();
            
            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = false,
                OmitXmlDeclaration = false,
                Encoding = System.Text.Encoding.GetEncoding(m_encoding)
            };

            using (var ms = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(ms, xmlWriterSettings))
                {
                    var ser = new XmlSerializer(typeof(T));
                    ser.Serialize(xmlWriter, obj);

                    info.SerializedBytes = ms.ToArray();

                    ms.Position = 0; // rewind the stream before reading back.
                    using (var sr = new StreamReader(ms))
                    {
                        info.SerializedString = sr.ReadToEnd();
                    }
                }
            }

            return info;
        }
    }
}
