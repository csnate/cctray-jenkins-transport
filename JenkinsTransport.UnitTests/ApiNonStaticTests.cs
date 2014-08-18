using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class ApiNonStaticTests
    {
        protected ApiNonStatic ApiInstance;
        protected string ProjectUrl = "https://builds.apache.org/job/Hadoop-1-win/api/xml";
        protected string ProjectName = "Hadoop-1-win";

        [TestInitialize]
        public void Setup()
        {
            ApiInstance = new ApiNonStatic("https://builds.apache.org/", String.Empty);
            
        }

        [TestMethod]
        public void Real()
        {
            ApiInstance.WebRequestFactory = new WebRequestFactory();
            var jobs = ApiInstance.GetAllJobs();
            Assert.IsNotNull(jobs);
            Assert.IsTrue(jobs.Any());
            CollectionAssert.AllItemsAreUnique(jobs);
        }

        [TestMethod]
        public void TestGetAllJobs()
        {
            Mock<IWebRequest> mockWebRequest = new Mock<IWebRequest>();

            Stream responseStream = new FileStream(@".\TestData\TestJobsSampleData1.xml", FileMode.Open);

            var webResponse = new TestWebReponse(responseStream);
            mockWebRequest
                .Setup(x => x.GetResponse())
                .Returns(webResponse);

            var mockWebRequestFactory = new Mock<IWebRequestFactory>();            
            mockWebRequestFactory
                .Setup(x => x.Create(It.IsAny<string>()))
                .Returns(mockWebRequest.Object);

            ApiInstance.WebRequestFactory = mockWebRequestFactory.Object;

            var jobs = ApiInstance.GetAllJobs();

            Assert.IsNotNull(jobs);
            Assert.IsTrue(jobs.Any());

            CollectionAssert.AllItemsAreUnique(jobs);
        }

    }


    internal class TestWebReponse : WebResponse
    {
        Stream responseStream;

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse(Stream responseStream)
        {
            this.responseStream = responseStream;
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream()
        {
            return responseStream;
        }
    }
}
