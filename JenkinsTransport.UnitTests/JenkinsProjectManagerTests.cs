using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using FluentAssertions;
using JenkinsTransport.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsProjectManagerTests
    {
        /// <summary>
        /// Organize required dependencies for this test target 
        /// </summary>
        internal class Mocks
        {
            /// <summary>
            /// Constructor creates instances of the mocks with a default behavior of loose
            /// </summary>
            /// <param name="mockBehavior"></param>
            public Mocks(MockBehavior mockBehavior = MockBehavior.Loose)
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>(mockBehavior);
                MockJenkinsApi = new Mock<IJenkinsApi>(mockBehavior);
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>(mockBehavior);
            }

            public Mock<IWebRequestFactory> MockWebRequestFactory;
            public IWebRequestFactory WebRequestFactory
            {
                get { return MockWebRequestFactory.Object; }
            }

            public Mock<IJenkinsApiFactory> MockJenkinsApiFactory;
            public IJenkinsApiFactory JenkinsApiFactory
            {
                get { return MockJenkinsApiFactory.Object; }
            }

            public Mock<IJenkinsApi> MockJenkinsApi;
            public IJenkinsApi JenkinsApi
            {
                get { return MockJenkinsApi.Object; }
            }
        }


        /// <summary>
        /// Create an instance of the subject under test (testTarget) with some default mock wiring in place
        /// </summary>
        /// <returns></returns>
        private JenkinsProjectManager CreateTestTarget(Mocks mocks)
        {
            return new JenkinsProjectManager(mocks.WebRequestFactory, mocks.JenkinsApiFactory);
        }

        private void SetupDefaultMockState(Mocks mocks)
        {
            mocks.MockJenkinsApiFactory
                .Setup(x => x.Create(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IWebRequestFactory>()))
                .Returns(mocks.JenkinsApi);
        }


        private static Settings GetDefaultTestSettings()
        {
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            return settings;
        }


        [TestMethod]
        public void Initialize_should_set_projectName()
        {
            Mocks mocks = new Mocks();
            var target = CreateTestTarget(mocks);

            var settings = GetDefaultTestSettings();
            var buildServer = new BuildServer(settings.Server);

            // Act
            target.Initialize(buildServer, "Hadoop-1-win", settings);

            // Assert
            target.ProjectName.Should().Be("Hadoop-1-win");       
        }

     

        [TestMethod]
        public void Initialize_should_set_settings()
        {
            Mocks mocks = new Mocks();
            var target = CreateTestTarget(mocks);

            var settings = GetDefaultTestSettings();
            var buildServer = new BuildServer(settings.Server);

            // Act
            target.Initialize(buildServer, "Hadoop-1-win", settings);

            // Assert
            target.Settings.Should().Be(settings);
        }

        [TestMethod]
        public void Initialize_should_set_authorizationInformation()
        {
            Mocks mocks = new Mocks();
            var target = CreateTestTarget(mocks);

            var settings = GetDefaultTestSettings();
            settings.AuthorizationInformation = "TestAuthorization";

            var buildServer = new BuildServer(settings.Server);

            // Act
            target.Initialize(buildServer, "Hadoop-1-win", settings);

            // Assert
            target.AuthorizationInformation.Should().Be("TestAuthorization");
        }

        [TestMethod]
        public void Initialize_should_set_configuration()
        {
            Mocks mocks = new Mocks();
            var target = CreateTestTarget(mocks);

            var settings = GetDefaultTestSettings();
            var buildServer = new BuildServer(settings.Server);

            // Act
            target.Initialize(buildServer, "Hadoop-1-win", settings);

            // Assert
            target.Configuration.Should().Be(buildServer);
        }


        [TestMethod]
        public void ForceBuild_when_using_weburl_should_call_api_with_parameters()
        {
            Mocks mocks = new Mocks();
            SetupDefaultMockState(mocks);

            var target = CreateTestTarget(mocks);
            
            target.WebURL = new Uri(@"http://test");

            target.Initialize(new BuildServer(), "TestProjectName", new Settings());

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"SomeParameter", "SomeValue"}
            };

            string userName = "TestUser";

            // Act
            target.ForceBuild("", parameters, userName);

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.ForceBuild(It.IsAny<Uri>(),
                    parameters),
                    Times.Once);
        }

        [TestMethod]
        public void AbortBuild_when_using_weburl_should_call_api()
        {
            Mocks mocks = new Mocks();
            SetupDefaultMockState(mocks);

            var target = CreateTestTarget(mocks);

            target.WebURL = new Uri(@"http://test");

            target.Initialize(new BuildServer(), "TestProjectName", new Settings());

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                {"SomeParameter", "SomeValue"}
            };

            string userName = "TestUser";
            string sessionToken = "";

            // Act
            target.AbortBuild(sessionToken, userName);

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.AbortBuild(target.WebURL),
                    Times.Once);
        }
    }
}