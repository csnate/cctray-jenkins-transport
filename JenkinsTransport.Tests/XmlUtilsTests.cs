using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public void TestGetXmlFromUrl()
        {
            var xmlDoc = XmlUtils.GetXmlFromUrl(BuildServer + Constants.AllProjects, AuthInfo);
            Assert.IsNotNull(xmlDoc);

            var jobs = xmlDoc.SelectNodes("/hudson/job");
            CollectionAssert.IsNotEmpty(jobs);
        }
    }
}
