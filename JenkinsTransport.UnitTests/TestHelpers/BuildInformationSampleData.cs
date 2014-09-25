using System.Linq;
using System.Xml.Linq;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    internal class BuildInformationSampleData
    {
        private XDocument _document;

        public XDocument Document
        {
            get { return _document; }
        }

        public void InitializeFromFile(string testdataBuildinformationsampledata1Xml)
        {
            _document = XDocument.Load(testdataBuildinformationsampledata1Xml);
        }

        public void SetBuildNumberTo(int i)
        {
            var firstElement = _document.Descendants().First<XElement>();
            //var lastBuildElement = firstElement.Element("lastBuild");

            firstElement.Element("number").Value = i.ToString();
        }
    }
}