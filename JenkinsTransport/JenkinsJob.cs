using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace JenkinsTransport
{
    [XmlRoot(ElementName = "job")]
    public class JenkinsJob
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }

        [XmlElement(ElementName = "color")]
        public string Color { get; set; }

        public static JenkinsJob GetJob(string xmlString)
        {
            if (String.IsNullOrEmpty(xmlString))
            {
                return new JenkinsJob();
            }

            var ser = new XmlSerializer(typeof(JenkinsJob));
            using (var rdr = new StringReader(xmlString))
            {
                return ser.Deserialize(rdr) as JenkinsJob;
            }
        }
    }
}
