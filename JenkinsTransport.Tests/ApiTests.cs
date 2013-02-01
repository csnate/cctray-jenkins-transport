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
    }
}
