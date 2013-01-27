using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
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
        protected JenkinsTransportExtension Transport;

        private IWin32Window GetForm()
        {
            return new Form();
        }

        [SetUp]
        public void SetUp()
        {
            Transport = new JenkinsTransportExtension {UseConfigurationFile = false};
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
            StringAssert.AreEqualIgnoringCase(Transport.Configuration.Url, "http://add.a.configuration.file.com");
            StringAssert.AreEqualIgnoringCase(Transport.Settings, String.Empty);
        }

        [Test]
        public void TestRetrieveServerManager()
        {
            Transport.Configure(GetForm());
            var manager = Transport.RetrieveServerManager();
            Assert.IsInstanceOf<JenkinsServerManager>(manager);
            StringAssert.AreEqualIgnoringCase(manager.DisplayName, Transport.Configuration.DisplayName);
            StringAssert.AreEqualIgnoringCase(manager.Configuration.Url, Transport.Configuration.Url);
            StringAssert.AreEqualIgnoringCase(manager.SessionToken, String.Empty);
        }

        [Test]
        public void TestRetrieveProjectManager()
        {
            Transport.Configure(GetForm());

            const string projectName = "Build Project";
            var manager = Transport.RetrieveProjectManager(projectName);
            Assert.IsInstanceOf<JenkinsProjectManager>(manager);
            StringAssert.AreEqualIgnoringCase(manager.ProjectName, projectName);
        }

        [Test]
        public void TestGetProjectList()
        {
            Transport.Configure(GetForm());
            var list = Transport.GetProjectList(Transport.Configuration);
            CollectionAssert.IsEmpty(list);
        }


        // Helper method to get the fully qualified name for an assembly
        [Test]
        [Explicit]
        public void TestAssemblyName()
        {
            Type t = typeof(JenkinsServerManagerTests);
            string s = t.Assembly.FullName.ToString();
            Console.WriteLine("The fully qualified assembly name is {0}.", s);
        }

        // --- Static Method Tests

        [Test]
        public void TestGetConfiguration()
        {
            var config = JenkinsTransportExtension.GetApplicationConfiguration();
            Assert.IsNotNull(config);
            CollectionAssert.IsNotEmpty(config.AppSettings.Settings);
        }

        [Test]
        public void TestGetBuildServerFromConfiguration()
        {
            var server = JenkinsTransportExtension.GetBuildServerFromConfiguration();
            Assert.IsNotNull(server);
            StringAssert.AreEqualIgnoringCase(server.Url, "http://build.office.comscore.com");
        }

        [Test]
        public void TestGetSettingsFromConfiguration()
        {
            var settings = JenkinsTransportExtension.GetSettingsFromConfiguration();
            Assert.IsNotNull(settings);
            StringAssert.AreEqualIgnoringCase(settings.Username, "cssuser");
            StringAssert.AreEqualIgnoringCase(settings.Password, "c0msc0r3");
            StringAssert.AreEqualIgnoringCase(settings.Server, "http://build.office.comscore.com");
            StringAssert.AreEqualIgnoringCase(settings.Project, String.Empty);
        }
    }
}
