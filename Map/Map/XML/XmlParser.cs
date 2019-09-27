using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Map.XML
{
    public class XmlParser
    {
        private string fileName;

        public XmlParser(string fileName)
        {
            this.fileName = fileName;
        }

        public void Serialize<T>(T obj)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());

                using(MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, obj);
                    stream.Position = 0;
                    doc.Load(stream);
                    doc.Save(fileName);
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public T DeSerialize<T>()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fileName);
                string xmlString = doc.OuterXml;

                using (StringReader reader = new StringReader(xmlString))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using(XmlReader xmlReader = new XmlTextReader(reader))
                    {
                        return (T)(serializer.Deserialize(xmlReader));
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
