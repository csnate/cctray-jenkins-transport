using System;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.UnitTests
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
            ApiInstance = new Api("https://builds.apache.org/", String.Empty);
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
            var status = ApiInstance.GetProjectStatus(ProjectUrl, null);
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
    
        // The following test cases need a custom implementation as we cannot perform any of these actions against the Apache Jenkins instance
        // Write your own test cases to perform these actions and confirm that they work correctly

        [TestMethod]
        [TestCategory("Custom")]
        public void TestForceBuild()
        {

        }

        [TestMethod]
        [TestCategory("Custom")]
        public void TestAbortBuild()
        {

        }

        [TestMethod]
        [TestCategory("Custom")]
        public void TestStopProject()
        {

        }

        [TestMethod]
        [TestCategory("Custom")]
        public void TestStartProject()
        {

        }

    }
}
