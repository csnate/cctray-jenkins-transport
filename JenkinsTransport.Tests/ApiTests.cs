using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class ApiTests
    {
        private Api api;

        [SetUp]
        public void SetUp()
        {
            api = new Api("http://build.office.comscore.com", Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", "cssuser", "c0msc0r3"))));
        }

        [Test]
        public void TestGetAllJobs()
        {
            var list = api.GetAllJobs();
            CollectionAssert.IsNotEmpty(list);
        }

        [Test]
        public void TestGetProjectStatusGood()
        {
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/Direct%20-%20INT/");
            Assert.IsNotNull(projectStatus);
        }

        [Test]
        public void TestGetProjectStatusDisabled()
        {
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/MyMetrix%20API%20-%20Regression/");
            Assert.IsNotNull(projectStatus);
        }
    }
}
