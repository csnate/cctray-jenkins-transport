using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.IntegrationTests
{
    [TestClass]
    public class JenkinsProjectManagerTests
    {
        protected JenkinsProjectManager Manager;

        [TestInitialize]
        public void Setup()
        {
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            var buildServer = new BuildServer(settings.Server);
            Manager = new JenkinsProjectManager(new WebRequestFactory());
            Manager.Initialize(buildServer, "Hadoop-1-win", settings);
        }

        [TestMethod]
        public void TestRetrieveSnapshot()
        {
            var snapshot = Manager.RetrieveSnapshot();
            Assert.AreEqual(snapshot.Name, "Hadoop-1-win");
        }

        [TestMethod]
        public void TestListBuildParameters()
        {
            var buildParameters = Manager.ListBuildParameters();
            Assert.IsTrue(buildParameters.Any());
        }
        
    }
}
