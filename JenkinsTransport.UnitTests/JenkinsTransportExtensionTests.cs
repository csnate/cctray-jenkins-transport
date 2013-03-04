using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;
using JenkinsTransport;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsTransportExtensionTests
    {
        private JenkinsTransportExtension Transport;

        [TestInitialize]
        public void Setup()
        {
            Transport = new JenkinsTransportExtension();
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            Transport.Settings = settings.ToString();
            Transport.UseConfigurationFile = false;
            Transport.Configuration = new BuildServer(settings.Server);
        }

        [TestMethod]
        public void TestJenkinsTransportExtensionInitialization()
        {
            Assert.IsNotNull(Transport.Settings);
            Assert.IsFalse(Transport.UseConfigurationFile);
            Assert.AreEqual(Transport.Configuration.Url, "https://builds.apache.org/");
        }

        [TestMethod]
        public void TestRetrieveServerManager()
        {
            var serverManager = Transport.RetrieveServerManager();
            Assert.IsInstanceOfType(serverManager, typeof(JenkinsServerManager));

            var jenkinsServerManager = (JenkinsServerManager)serverManager;
            Assert.AreEqual(Transport.Configuration, jenkinsServerManager.Configuration);
            Assert.AreEqual(jenkinsServerManager.SessionToken, String.Empty);
            Assert.IsFalse(jenkinsServerManager.ProjectsAndCurrentStatus.Any());
        }

        [TestMethod]
        public void TestRetrieveProjectManager()
        {
            var projectManager = Transport.RetrieveProjectManager("Test Project");
            Assert.IsInstanceOfType(projectManager, typeof(JenkinsProjectManager));

            var jenkinsProjectManager = (JenkinsProjectManager)projectManager;
            Assert.AreEqual(Transport.Configuration, jenkinsProjectManager.Configuration);
            Assert.AreEqual(jenkinsProjectManager.ProjectName, "Test Project");
            Assert.IsNotNull(jenkinsProjectManager.AuthorizationInformation);

            Assert.IsTrue(((JenkinsServerManager)Transport.RetrieveServerManager()).ProjectsAndCurrentStatus.Any());
        }

        [TestMethod]
        public void TestConfigureWithNoConfigurationFile()
        {
            Transport.Configure(new Form());
            Assert.AreEqual(Transport.Settings, String.Empty);
            Assert.AreEqual(Transport.Configuration.Url, "http://add.a.configuration.file.com");
        }

        [TestMethod]
        public void TestConfigureWithConfigurationFile()
        {
            // Copy the config file from JenkinsTransport to this project
            Transport.UseConfigurationFile = true;
            Transport.Configure(new Form());
            Assert.IsNotNull(Transport.Settings);
            Assert.AreEqual(Transport.Configuration.Url, "http://my.build.server.com");
        }

        [TestMethod]
        public void TestGetProjectList()
        {
            var projectList = Transport.GetProjectList(Transport.Configuration).ToList();
            Assert.IsTrue(projectList.Any());
            CollectionAssert.AllItemsAreUnique(projectList);
        }
    }
}
