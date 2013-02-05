using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class XmlUtilsTests
    {
        private string BuildServer;
        private string AuthInfo;

        [SetUp]
        public void SetUp()
        {
            BuildServer = "http://build.office.comscore.com";
            AuthInfo = Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", "cssuser", "c0msc0r3")));
        }

        [Test]
        public void TestGetXDocumentFromUrl()
        {
            var xmlDoc = XmlUtils.GetXDocumentFromUrl(BuildServer + "/api/xml", AuthInfo);
            Assert.IsNotNull(xmlDoc);
            Assert.That(xmlDoc.Elements().Count(), Is.GreaterThan(0));

            var elements = xmlDoc.Element("hudson").Elements("job");
            Assert.That(elements.Count(), Is.EqualTo(5));
        }
    }
}
