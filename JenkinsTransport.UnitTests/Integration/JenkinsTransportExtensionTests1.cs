using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests.Integration
{
    [TestClass]
    public class JenkinsTransportExtensionTests1
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
            Transport.Configuration = new BuildServer(settings.Server);
        }
       
        [TestMethod]
        public void TestGetProjectList()
        {
            var projectList = Transport.GetProjectList(Transport.Configuration).ToList();
            Assert.IsTrue(projectList.Any());
            CollectionAssert.AllItemsAreUnique(projectList);
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
    }
}
