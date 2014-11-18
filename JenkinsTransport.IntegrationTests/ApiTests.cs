using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.IntegrationTests
{
    [TestClass]
    public class ApiTests
    {
        protected Api ApiInstance;
        protected string ProjectUrl = "https://builds.apache.org/job/Hadoop-1-win/api/xml";
        protected string ProjectName = "Hadoop-1-win";

        [TestInitialize]
        public void Setup()
        {
            ApiInstance = new Api("https://builds.apache.org/", String.Empty, new WebRequestFactory());
        }

        [TestMethod]
        public void TestGetAllJobs()
        {
            var jobs = ApiInstance.GetAllJobs();
            Assert.IsNotNull(jobs);
            Assert.IsTrue(jobs.Any());
            CollectionAssert.AllItemsAreUnique(jobs);
        }

        [TestMethod]
        public void TestGetProjectStatus()
        {
            ProjectStatus status = ApiInstance.GetProjectStatus(ProjectUrl, null);
            Assert.IsNotNull(status);
            Assert.AreEqual(status.Name, ProjectName);
            StringAssert.Contains(status.WebURL, "https://builds.apache.org/job/Hadoop-1-win");
        }

        [TestMethod]
        public void TestGetBuildInformation()
        {
            var status = ApiInstance.GetProjectStatus(ProjectUrl, null);
            var buildInformation = ApiInstance.GetBuildInformation(status.WebURL + status.LastBuildLabel + "/");
            Assert.IsNotNull(buildInformation);
            Assert.AreEqual(buildInformation.Number, status.LastBuildLabel);
            StringAssert.Contains(buildInformation.FullDisplayName, ProjectName);
        }

        [TestMethod]
        public void TestGetProjectStatusSnapshot()
        {
            var snapshot = ApiInstance.GetProjectStatusSnapshot(ProjectName);
            Assert.IsNotNull(snapshot);
            Assert.AreEqual(snapshot.Name, ProjectName);
            Assert.AreEqual(snapshot.Error, String.Empty);
        }

        [TestMethod]
        public void TestGetBuildParameters()
        {
            var buildParameters = ApiInstance.GetBuildParameters(ProjectName);

            Assert.IsTrue(buildParameters.Any());
            CollectionAssert.AllItemsAreInstancesOfType(buildParameters, typeof(ParameterBase));
        }      
    }
}
