using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;
using FluentAssertions;
using JenkinsTransport;
using JenkinsTransport.Interface;
using JenkinsTransport.UnitTests.ExtensionMethods;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsTransportExtensionTests
    {
        // Because this class contains a static that must be initialized for repeatable/non dependent test runs
        // the tests can only run in single threaded mode otherwise a running test can be corrupted by a another 
        // test setting the static to null
        // Thus we must use a static lock object (static beause MsTest creates new instances of the test fixture for
        // each thread) so that all threads are locking on the same object before a test is run
        private static object syncLock = new object();

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

            public Mock<IJenkinsServerManagerFactory> MockJenkinsServerManagerFactory { get; set; }
            public IJenkinsServerManagerFactory JenkinsServerManagerFactory
            {
                get { return MockJenkinsServerManagerFactory.Object; }
            }

            public Mock<IJenkinsServerManager> MockJenkinsServerManager { get; set; }
            public IJenkinsServerManager JenkinsServerManager { get { return MockJenkinsServerManager.Object; } }

            public Mock<IFormFactory> MockConfigurationFormFactory { get; set; }

            public IFormFactory ConfigurationFormFactory
            {
                get
                {
                    return MockConfigurationFormFactory.Object;
                }
            }

            public TestMocks()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
                MockApi = new Mock<IJenkinsApi>();
                MockJenkinsServerManagerFactory = new Mock<IJenkinsServerManagerFactory>();
                MockJenkinsServerManager = new Mock<IJenkinsServerManager>();
                MockConfigurationFormFactory = new Mock<IFormFactory>();

                MockJenkinsServerManager.As<ICruiseServerManager>();

                // Default configuration for ApiFactory is to return this mock
                MockJenkinsApiFactory
                    .Setup(x => x.Create(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IWebRequestFactory>()))
                    .Returns(Api);
                
                // Default configuration for unit tests is to return a new instance of JenkinsServerManager rather than the static singleton
                MockJenkinsServerManagerFactory
                    .Setup(x => x.GetInstance())
                    .Returns(JenkinsServerManager);

            }
        }

       
        [TestInitialize]
        public void Setup()
        {
            Monitor.Enter(syncLock);
        }

        [TestCleanup]
        public void Teardown()
        {
            Monitor.Exit(syncLock);
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

            Transport.JenkinsServerManagerFactory = mocks.JenkinsServerManagerFactory;
            Transport.SetIsServerManagerInitialized(false);
            Transport.WebRequestFactory = mocks.WebRequestFactory;
            Transport.JenkinsApiFactory = mocks.JenkinsApiFactory;
            Transport.ConfigurationFormFactory = mocks.ConfigurationFormFactory;

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

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

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
            
            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

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

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

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

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

            // Act
            var projectManager = (JenkinsProjectManager)target.RetrieveProjectManager("Test Project");

            // Assert
            projectManager.AuthorizationInformation.Should().NotBeNull();
        }
       
        [TestMethod]
        public void RetrieveServerManager_should_access_factory_singleton_instance()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

            // Act
            target.RetrieveServerManager();

            // Assert
            mocks.MockJenkinsServerManagerFactory
                .Verify(x => x.GetInstance(),
                    Times.AtLeastOnce);
        }

        [TestMethod]
        public void RetrieveServerManager_should_initialize_serverManager()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

            // Act
            target.RetrieveServerManager();

            // Assert
            mocks.MockJenkinsServerManager
                .Verify(x => x.Initialize(It.IsAny<BuildServer>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                    Times.Once);
        }

        /// <summary>
        /// Verify that showing the configuration window will cause the ServerManager to be initialized next time
        /// the interface method RetrieveServerManager is called
        /// </summary>
        [TestMethod]
        public void RetrieveServerManager_when_configure_window_has_been_shown_should_initialze_serverManager()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            target.SetIsServerManagerInitialized(true);

            Mock<IForm> mockConfigurationForm = new Mock<IForm>();
            mockConfigurationForm
                .Setup(x => x.ShowDialog(It.IsAny<IWin32Window>()))
                .Returns(DialogResult.OK);

            mockConfigurationForm
                .Setup(x => x.GetServer())
                .Returns(@"https://SeomServer.com");

            mocks.MockConfigurationFormFactory
                .Setup(x => x.Create())
                .Returns(mockConfigurationForm.Object);

            target.Configure(null);

            // Act
            target.RetrieveServerManager();

            // Assert
            mocks.MockJenkinsServerManager
                .Verify(x => x.Initialize(It.IsAny<BuildServer>(),
                    It.IsAny<string>(), It.IsAny<string>()),
                    Times.Once);
        }

    }
}
