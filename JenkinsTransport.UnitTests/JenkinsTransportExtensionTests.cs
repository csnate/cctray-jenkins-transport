using FluentAssertions;
using JenkinsTransport.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
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

            public Mock<IDialogService> MockDialogService { get; set; }
            public IDialogService DialogService { get { return MockDialogService.Object; } }

            public TestMocks()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
                MockApi = new Mock<IJenkinsApi>();
                MockJenkinsServerManagerFactory = new Mock<IJenkinsServerManagerFactory>();
                MockJenkinsServerManager = new Mock<IJenkinsServerManager>();
                MockJenkinsServerManager.As<ICruiseServerManager>();
                MockDialogService = new Mock<IDialogService>();

                MockConfigurationFormFactory = new Mock<IFormFactory>();

                // Default configuration for ApiFactory is to return this mock
                MockJenkinsApiFactory
                    .Setup(x => x.Create(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IWebRequestFactory>()))
                    .Returns(Api);
                
                // Default configuration for unit tests is to return the JenkinsServerManager mocks
                MockJenkinsServerManagerFactory
                    .Setup(x => x.GetInstance())
                    .Returns(JenkinsServerManager);

            }
        }

       
        [TestInitialize]
        public void Setup()
        {
        }

        [TestCleanup]
        public void Teardown()
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

            Transport.JenkinsServerManagerFactory = mocks.JenkinsServerManagerFactory;
            Transport.WebRequestFactory = mocks.WebRequestFactory;
            Transport.JenkinsApiFactory = mocks.JenkinsApiFactory;
            Transport.ConfigurationFormFactory = mocks.ConfigurationFormFactory;
            Transport.DialogService = mocks.DialogService;

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
        public void RetrieveProjectManager_when_projectName_does_not_exist_in_serverManager_should_be_added()
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
            mocks.JenkinsServerManager.ProjectsAndCurrentStatus.Should()
                .Contain(new KeyValuePair<string, ProjectStatus>("Test Project", null));
        }
        
        [TestMethod]
        public void RetrieveProjectManager_when_projectName_does_not_have_current_status_should_get_server_snapshot()
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
            mocks.MockJenkinsServerManager
                .Verify(x => x.GetCruiseServerSnapshotEx(),
                Times.Once);
        }

        [TestMethod]
        public void RetrieveProjectManager_when_projectName_already_has_current_status_should_not_get_server_snapshot()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            projectsAndCurrentStatus.Add("Test Project", new ProjectStatus());

            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

            // Act
            var projectManager = target.RetrieveProjectManager("Test Project");

            // Assert
            mocks.MockJenkinsServerManager
                .Verify(x => x.GetCruiseServerSnapshot(),
                Times.Never);
        }

        [TestMethod]
        public void RetrieveProjectManager_when_project_status_has_valid_webUrl_projectManager_should_use_it()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            List<JenkinsJob> allJobs = new List<JenkinsJob>();

            mocks.MockApi
                .Setup(x => x.GetAllJobs())
                .Returns(allJobs);

            Dictionary<string, ProjectStatus> projectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
            projectsAndCurrentStatus.Add("Test Project", null);

            mocks.MockJenkinsServerManager
                .Setup(x => x.ProjectsAndCurrentStatus)
                .Returns(projectsAndCurrentStatus);

            mocks.MockJenkinsServerManager
                .Setup(x => x.GetCruiseServerSnapshotEx())
                .Callback(() =>
                {
                    // Simulate the project status being updated as per normal
                    projectsAndCurrentStatus["Test Project"] = new ProjectStatus();
                    projectsAndCurrentStatus["Test Project"].WebURL = @"http://SomeTestServer5.com";
                });
            
            // Act
            var projectManager = target.RetrieveProjectManager("Test Project");

            // Assert
            ((JenkinsProjectManager) projectManager).WebURL.Should().Be(@"http://SomeTestServer5.com");
        }

        [TestMethod]
        public void RetrieveProjectManager_instance_should_be_initialized_with_current_configuration()
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
        public void RetrieveProjectManager_instance_should_be_initialized_with_current_settings()
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
            target.Settings.Should().Be(projectManager.Settings.ToString());
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
        public void RetrieveProjectManager_when_server_unavailable_should_display_dialog()
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

            mocks.MockJenkinsServerManager
                .Setup(x => x.GetCruiseServerSnapshotEx())
                .Throws(new WebException("Test WebException Message"));

            // Act
            var projectManager = target.RetrieveProjectManager("Test Project");

            // Assert
            mocks.MockDialogService
                .Verify(x => x.Show("Test WebException Message"));
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
        public void RetrieveServerManager_when_configure_window_has_been_shown_should_set_isInitialized_to_false()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);
           
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
            
            // Act
            target.Configure(null);

            // Assert
            mocks.MockJenkinsServerManagerFactory
                .VerifySet(x => x.IsServerManagerInitialized = false);
        }

        [TestMethod]
        public void RetrieveServerManager_when_server_has_already_been_configured_should_display_warning()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

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

            mocks.MockJenkinsServerManagerFactory
                .Setup(x => x.IsServerManagerInitialized)
                .Returns(true);

            // record the value from the dialog for assertion
            string displayedDialogText = String.Empty;
            mocks.MockDialogService
                .Setup(x => x.Show(It.IsAny<string>()))
                .Callback<string>((dialogText) => { displayedDialogText = dialogText; });

            // Act
            target.Configure(null);

            // Assert
            const string expectedDialogText = "Monitoring jobs from multiple jenkins servers is unsupported due to the CCTray interface.\n" +
                                              "If you have previously removed a jenkins server, restart CCTray first.";
            displayedDialogText.Should().Be(expectedDialogText);
        }

    }
}
