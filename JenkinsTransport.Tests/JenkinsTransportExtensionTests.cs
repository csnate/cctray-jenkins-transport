using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsTransportExtensionTests
    {
        protected ITransportExtension Transport;

        private IWin32Window GetForm()
        {
            return new Form();
        }

        [SetUp]
        public void SetUp()
        {
            Transport = new JenkinsTransportExtension();
        }

        [Test]
        public void TestInstanciation()
        {
            Assert.IsNotNull(Transport);
            StringAssert.AreEqualIgnoringCase(Transport.DisplayName, "Jenkins Transport Extension");
        }

        [Test]
        public void TestConfigure()
        {
            var response = Transport.Configure(GetForm());
            Assert.IsTrue(response);
            Assert.IsInstanceOf<BuildServer>(Transport.Configuration);
            StringAssert.AreEqualIgnoringCase(Transport.Configuration.Url, "http://build.office.comscore.com");

            var settings = Settings.GetSettings(Transport.Settings);
            StringAssert.AreEqualIgnoringCase(settings.Project, "Build");
            StringAssert.AreEqualIgnoringCase(settings.Server, "CVIADPZB02");
        }

        [Test]
        public void TestRetrieveServerManager()
        {
            var manager = Transport.RetrieveServerManager();
            Assert.IsInstanceOf<JenkinsServerManager>(manager);
        }

        [Test]
        public void TestRetrieveProjectManager()
        {
            var manager = Transport.RetrieveProjectManager(String.Empty);
            Assert.IsInstanceOf<JenkinsProjectManager>(manager);
        }

        [Test]
        public void TestGetProjectList()
        {
            Transport.Configure(GetForm());
            var list = Transport.GetProjectList(Transport.Configuration);
            CollectionAssert.IsEmpty(list);
        }
    }
}
