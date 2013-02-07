using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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

        public JenkinsJob() {}

        public JenkinsJob(XContainer element)
        {
            Name = (string) element.Element("name");
            Url = (string) element.Element("url");
            Color = (string) element.Element("color");
        }
    }
}
