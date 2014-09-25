using System.Linq;
using System.Xml.Linq;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    /// <summary>
    /// Helper class for setting up test state of ProjectStatusSampleData
    /// </summary>
    internal class ProjectStatusSampleData
    {
        private XDocument _document;

        public XDocument Document
        {
            get { return _document; }
        }

        public void InitializeFromFile(string testdataProjectstatussampledata1Xml)
        {
            _document = XDocument.Load(testdataProjectstatussampledata1Xml);
        }

        public void SetLastBuildNumberTo(int i)
        {
            var firstElement = _document.Descendants().First<XElement>(); 
            var lastBuildElement = firstElement.Element("lastBuild");

            lastBuildElement.Element("number").Value = i.ToString();

        }

        public void SetLastCompletedBuildUrlTo(string value)
        {
            var firstElement = _document.Descendants().First<XElement>();
            var lastBuildElement = firstElement.Element("lastCompletedBuild");

            lastBuildElement.Element("url").Value = value;
        }

        public void RemoveLastCompletedBuildElements()
        {
            var firstElement = _document.Descendants().First<XElement>();
            var lastBuildElement = firstElement.Element("lastCompletedBuild");

            lastBuildElement.Remove();
        }

        public void SetLastCompletedBuildNumberTo(int value)
        {
            var firstElement = _document.Descendants().First<XElement>();
            var lastBuildElement = firstElement.Element("lastCompletedBuild");

            lastBuildElement.Element("number").Value = value.ToString();
        }

        public void SetLastSuccessfulBuildNumberTo(int value)
        {
            var firstElement = _document.Descendants().First<XElement>();
            var lastBuildElement = firstElement.Element("lastSuccessfulBuild");

            lastBuildElement.Element("number").Value = value.ToString();
        }
    }
}