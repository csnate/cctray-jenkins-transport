using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.IntegrationTests
{
    [TestClass]
    public class JenkinsServerManagerTests
    {
        protected JenkinsServerManager Manager;

        [TestInitialize]
        public void Setup()
        {
            Manager = new JenkinsServerManager(new WebRequestFactory(), new JenkinsApiFactory(), new DateTimeService());
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            var buildServer = new BuildServer(settings.Server);
            Manager.Initialize(buildServer, String.Empty, settings);
        }
   
        [TestMethod]
        public void TestGetProjectList()
        {
            var projectList = Manager.GetProjectList();
            Assert.IsTrue(projectList.Any());
        }

        [TestMethod]
        public void TestGetCruiseServerSnapshot()
        {
            var projectList = Manager.GetProjectList();
            Manager.ProjectsAndCurrentStatus.Add(projectList[0].ProjectName, null);

            var snapshot = Manager.GetCruiseServerSnapshot();
            Assert.IsNotNull(snapshot);
            Assert.IsTrue(snapshot.ProjectStatuses.Any());
            Assert.IsTrue(snapshot.ProjectStatuses.Count() == 1);
        }

        
    }
}
