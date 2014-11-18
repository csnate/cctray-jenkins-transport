using System.Xml.Linq;

namespace JenkinsTransport
{
    public class JenkinsJob
    {
        public string Name { get; set; }
        public string Url { get; set; }
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
