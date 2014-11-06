using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using FluentAssertions;
using JenkinsTransport;
using JenkinsTransport.Interface;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsTransportExtensionTests
    {
        internal class TestMocks
        {
            public Mock<IWebRequestFactory> MockWebRequestFactory { get; set; }
            public Mock<IJenkinsApiFactory> MockJenkinsApiFactory { get; set; }
            public Mock<IJenkinsApi> MockApi;
            public IJenkinsApi Api
            {
                get
                {
                    return MockApi.Object;
                }
            }
            
            public IWebRequestFactory WebRequestFactory { get { return MockWebRequestFactory.Object; } }
            public IJenkinsApiFactory JenkinsApiFactory { get { return MockJenkinsApiFactory.Object; }}

            public TestMocks()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
                MockApi = new Mock<IJenkinsApi>();

                // Default configuration for ApiFactory is to return this mock
                MockJenkinsApiFactory
                    .Setup(x => x.Create(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IWebRequestFactory>()))
                    .Returns(Api);

            }
        }

        [TestInitialize]
        public void Setup()
        {
        }

        private JenkinsTransportExtension CreateTestTarget(TestMocks mocks)
        {
            var Transport = new JenkinsTransportExtension();

            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            Transport.WebRequestFactory = mocks.WebRequestFactory;
            Transport.JenkinsApiFactory = mocks.JenkinsApiFactory;
            Transport.Settings = settings.ToString();
            Transport.Configuration = new BuildServer(settings.Server);

            return Transport;
        }


        [TestMethod]
        public void Settings_setter_should_update_settings()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            var settings = new Settings()
            {
                Project = "SomeProjectName",
                Username = "SomeUserName",
                Password = "SomePassword",
                Server = "https://some.testserver.com/"
            };

            // Act
            target.Settings = settings.ToString();

            // Assert
            target.Settings.Should().Be(settings.ToString());
        }

        [TestMethod]
        public void Configuration_setter_should_update_configuration()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            // Act
            target.Configuration = new BuildServer("https://some.othertest.server.com/");

            // Assert
            Assert.AreEqual(target.Configuration.Url, "https://some.othertest.server.com/");
        }

        [TestMethod]
        public void RetrieveProjectManager_should_return_instance_of_JenkinsProjectManager()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            // Act
            var projectManager = target.RetrieveProjectManager("Test Project");

            // Assert
            projectManager.Should().BeAssignableTo<JenkinsProjectManager>();
        }

        [TestMethod]
        public void RetrieveProjectManager_instance_should_use_configuration()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            // Act
            var projectManager = (JenkinsProjectManager) target.RetrieveProjectManager("Test Project");
            
            // Assert
            target.Configuration.Should().Be(projectManager.Configuration);
        }

        [TestMethod]
        public void RetrieveProjectManager_instance_should_use_supplied_projectName()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            // Act
            var projectManager = (JenkinsProjectManager)target.RetrieveProjectManager("Test Project");

            // Assert
            projectManager.ProjectName.Should().Be("Test Project");
        }

        [TestMethod]
        public void RetrieveProjectManager_instance_AuthorizationInformation_should_not_be_null()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            // Act
            var projectManager = (JenkinsProjectManager)target.RetrieveProjectManager("Test Project");

            // Assert
            projectManager.AuthorizationInformation.Should().NotBeNull();
        }

        [TestMethod]
        [Ignore]
        // Test currently not compatible with static ServerManager - TODO NJ - Provide extension method to set static
        public void RetrieveProjectManager_when_project_does_not_exist_already_should_add_to_dictionary()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>()
            {
                new JenkinsJob() {Name = "Test Project"}
            };

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            mocks.MockApi
                .Setup(x => x.GetProjectStatus(
                    It.IsAny<string>(),
                    It.IsAny<ProjectStatus>()))
                .Returns(new ProjectStatus() { Name = "Test Project" });

            // Act
            target.RetrieveProjectManager("Test Project");

            // Assert
            target.RetrieveServerManager()
                .As<IJenkinsServerManager>()
                .ProjectsAndCurrentStatus.Should()
                .ContainKey("Test Project");
        }

        [TestMethod]
        public void TestRetrieveServerManager()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            var serverManager = target.RetrieveServerManager();
            
            Assert.IsInstanceOfType(serverManager, typeof(JenkinsServerManager));

            var jenkinsServerManager = (JenkinsServerManager)serverManager;

            Assert.AreEqual(target.Configuration, jenkinsServerManager.Configuration);
            Assert.AreEqual(jenkinsServerManager.SessionToken, String.Empty);

            // This assert is disabled as there is a static conflict with the TestRetrieveProjectManager test
            //Assert.IsFalse(jenkinsServerManager.ProjectsAndCurrentStatus.Any());
        }
    }
}
