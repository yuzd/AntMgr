using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Infrastructure.StaticExt
{
    public static class Serializer
    {
        public static T DeserializeXml<T>(string xmlFilePath) where T : class
        {
            using (StreamReader sr = new StreamReader(xmlFilePath))
            {
                XmlSerializer xmldes = new XmlSerializer(typeof(T));
                return xmldes.Deserialize(sr) as T;
            }
        }
    }
}
