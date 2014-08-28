using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests
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
        public void TestInstanciation()
        {
            Assert.AreEqual(Manager.ProjectName, "Hadoop-1-win");
            Assert.AreEqual(Manager.AuthorizationInformation, String.Empty);
            Assert.AreEqual(Manager.Configuration.Url, "https://builds.apache.org/");
            Assert.AreEqual(Manager.Settings.Server, "https://builds.apache.org/");
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
