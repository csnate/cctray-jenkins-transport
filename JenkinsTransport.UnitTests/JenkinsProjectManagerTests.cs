using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using JenkinsTransport.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsProjectManagerTests
    {
        protected JenkinsProjectManager Manager;

        /// <summary>
        /// Create an instance of the subject under test (testTarget) with some default mock wiring in place
        /// </summary>
        /// <param name="mockWebRequestFactory"></param>
        /// <param name="mockJenkinsApiFactory"></param>
        /// <param name="mockApi"></param>
        /// <returns></returns>
        private JenkinsProjectManager CreateTestTarget(out Mock<IWebRequestFactory> mockWebRequestFactory, out Mock<IJenkinsApiFactory> mockJenkinsApiFactory, out Mock<IJenkinsApi> mockApi)
        {
            mockWebRequestFactory = new Mock<IWebRequestFactory>();
            mockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
            mockApi = new Mock<IJenkinsApi>();

            mockJenkinsApiFactory
             .Setup(x => x.Create(
                 It.IsAny<string>(),
                 It.IsAny<string>(),
                 It.IsAny<IWebRequestFactory>()))
             .Returns(mockApi.Object);

            return new JenkinsProjectManager(mockWebRequestFactory.Object, mockJenkinsApiFactory.Object);
        }

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
            Manager = new JenkinsProjectManager(new WebRequestFactory(), new JenkinsApiFactory());
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
        public void ForceBuild_when_using_weburl_should_call_api_with_parameters()
        {
            Mock<IWebRequestFactory> mockWebRequestFactory;
            Mock<IJenkinsApiFactory> mockJenkinsApiFactory;
            Mock<IJenkinsApi> mockApi;
         
            var target = CreateTestTarget(out mockWebRequestFactory, out mockJenkinsApiFactory, out mockApi);
            target.WebURL = new Uri(@"http://test");

            target.Initialize(new BuildServer(), "TestProjectName", new Settings() );

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"SomeParameter", "SomeValue"}
            };
            
            string userName = "TestUser";

            // Act
            target.ForceBuild("", parameters, userName);

            // Assert
            mockApi
                .Verify(x => x.ForceBuild(It.IsAny<Uri>(),
                    parameters),
                    Times.Once);
        }

    
    }
}
