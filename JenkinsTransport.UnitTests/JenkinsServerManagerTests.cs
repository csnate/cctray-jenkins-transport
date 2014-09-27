using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using FluentAssertions;
using JenkinsTransport.Interface;
using JenkinsTransport.UnitTests.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsServerManagerTests
    {
        internal class JenkinsServerManagerMocks
        {
            public Mock<IWebRequestFactory> MockWebRequestFactory;
            public Mock<IJenkinsApiFactory> MockJenkinsApiFactory;
            public Mock<IJenkinsApi> MockJenkinsApi;
            public Mock<IDateTimeService> MockDateTimeService;

            public JenkinsServerManagerMocks()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
                MockJenkinsApi = new Mock<IJenkinsApi>();
                MockDateTimeService = new Mock<IDateTimeService>();

                MockJenkinsApiFactory
                    .Setup(x => x.Create(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<IWebRequestFactory>()))
                    .Returns(MockJenkinsApi.Object);

            }
        }

        protected JenkinsServerManager Manager;

        /// <summary>
        /// Configure some existing cached jobs
        /// </summary>
        /// <param name="target"></param>
        private void ConfigureExistingCachedJobs(JenkinsServerManager target)
        {
            List<JenkinsJob> allJobs = new List<JenkinsJob>()
            {
                new JenkinsJob()
                {
                    Color = "Red",
                    Name = "TestJob1",
                    Url = "http:\\SomeUrl"
                },
                new JenkinsJob()
                {
                    Color = "Blue",
                    Name = "TestJob2",
                    Url = "http:\\SomeUrl2"
                },
            };

            target.SetAllJobs(allJobs);
        }

        private JenkinsServerManager CreateTestTarget(JenkinsServerManagerMocks mocks)
        {
            return CreateTestTarget(mocks.MockWebRequestFactory, mocks.MockJenkinsApiFactory,
                mocks.MockDateTimeService);
        }

        private JenkinsServerManager CreateTestTarget(Mock<IWebRequestFactory> mockWebRequestFactory, Mock<IJenkinsApiFactory> mockJenkinsApiFactory, Mock<IDateTimeService> mockDateTimeService)
        {
            return new JenkinsServerManager(mockWebRequestFactory.Object, mockJenkinsApiFactory.Object, mockDateTimeService.Object);
        }

        [TestInitialize]
        public void Setup()
        {
            Manager = new JenkinsServerManager(new WebRequestFactory(), new JenkinsApiFactory(), new DateTimeService());
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = String.Empty,
                Password = String.Empty,
                Server = "https://builds.apache.org/"
            };
            var buildServer = new BuildServer(settings.Server);
            Manager.Initialize(buildServer, String.Empty, settings);
        }

        [TestMethod]
        public void TestInstanciation()
        {
            Assert.IsNotNull(Manager);
            Assert.IsNotNull(Manager.Settings);
            Assert.AreEqual(Manager.AuthorizationInformation, String.Empty);
            Assert.IsFalse(Manager.ProjectsAndCurrentStatus.Any());
        }

        [TestMethod]
        public void TestSetConfiguration()
        {
            var buildServer = new BuildServer("http://test.com");
            Manager.SetConfiguration(buildServer);
            Assert.AreEqual(Manager.Configuration.Url, "http://test.com");
        }
        
        [TestMethod]
        public void TestLoginAndLogout()
        {
            var settings = new Settings()
            {
                Project = String.Empty,
                Username = "test",
                Password = "test",
                Server = "https://builds.apache.org/"
            };
            var buildServer = new BuildServer(settings.Server);
            Manager.Initialize(buildServer, String.Empty, settings);
            Manager.Login();

            Assert.IsFalse(String.IsNullOrEmpty(Manager.AuthorizationInformation));

            Manager.Logout();

            Assert.IsTrue(String.IsNullOrEmpty(Manager.AuthorizationInformation));
        }

        [TestMethod]
        public void GetProjectList_when_cache_expired_should_retrieve_all_jobs_from_api()
        {
            JenkinsServerManagerMocks mocks = new JenkinsServerManagerMocks();
            var target = CreateTestTarget(mocks);

            target.Initialize(new BuildServer(), "", "");

            ConfigureExistingCachedJobs(target);
            target.AllJobsLastUpdate = DateTime.Parse("10-Jan-2014 10:00:00");

            mocks.MockDateTimeService
                .Setup(x => x.Now)
                .Returns(DateTime.Parse("10-Jan-2014 10:00:03"));

            List<JenkinsJob> jobsFromWeb = new List<JenkinsJob>()
            {
                new JenkinsJob()
                {
                    Color = "Green",
                    Name = "TestJob3",
                    Url = "http:\\someUrl3"
                }
            };

            mocks.MockJenkinsApi
                .Setup(x => x.GetAllJobs())
                .Returns(jobsFromWeb);

            // Act
            target.GetProjectList();

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.GetAllJobs(),
                Times.Once);
        }

        [TestMethod]
        public void GetProjectList_when_cache_expired_should_update_cache_timestamp()
        {
            JenkinsServerManagerMocks mocks = new JenkinsServerManagerMocks();
            var target = CreateTestTarget(mocks);

            target.Initialize(new BuildServer(), "", "");

            ConfigureExistingCachedJobs(target);
            target.AllJobsLastUpdate = DateTime.Parse("10-Jan-2014 10:00:00");

            mocks.MockDateTimeService
                .Setup(x => x.Now)
                .Returns(DateTime.Parse("10-Jan-2014 10:00:03"));

            List<JenkinsJob> jobsFromWeb = new List<JenkinsJob>()
            {
                new JenkinsJob()
                {
                    Color = "Green",
                    Name = "TestJob3",
                    Url = "http:\\someUrl3"
                }
            };

            mocks.MockJenkinsApi
                .Setup(x => x.GetAllJobs())
                .Returns(jobsFromWeb);

            // Act
            target.GetProjectList();

            // Assert
            target.AllJobsLastUpdate.Should().Be(DateTime.Parse("10-Jan-2014 10:00:03"));
        }

        [TestMethod]
        public void GetProjectList_when_cache_not_expired_should_not_retrieve_all_jobs_from_api()
        {
            JenkinsServerManagerMocks mocks = new JenkinsServerManagerMocks();
            var target = CreateTestTarget(mocks);

            target.Initialize(new BuildServer(), "", "");

            ConfigureExistingCachedJobs(target);
            target.AllJobsLastUpdate = DateTime.Parse("10-Jan-2014 10:00:00");

            mocks.MockDateTimeService
                .Setup(x => x.Now)
                .Returns(DateTime.Parse("10-Jan-2014 10:00:01"));
                                  
            // Act
            target.GetProjectList();

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.GetAllJobs(),
                Times.Never);
        }

        [TestMethod]
        public void GetCruiseServerSnapshot_when_cache_is_popualted_should_not_retrieve_jobs_from_api()
        {
            JenkinsServerManagerMocks mocks = new JenkinsServerManagerMocks();
            var target = CreateTestTarget(mocks);

            target.Initialize(new BuildServer(), "", "");

            ConfigureExistingCachedJobs(target);
            target.AllJobsLastUpdate = DateTime.Parse("10-Jan-2014 10:00:00");           

            // Act
            target.GetCruiseServerSnapshot();

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.GetAllJobs(),
                Times.Never);

        }

        [TestMethod]
        public void GetCruiseServerSnapshot_when_cache_is_null_should_retrieve_all_jobs_from_api()
        {
            JenkinsServerManagerMocks mocks = new JenkinsServerManagerMocks();
            var target = CreateTestTarget(mocks);

            target.Initialize(new BuildServer(), "", "");
          
            target.AllJobsLastUpdate = DateTime.Parse("10-Jan-2014 10:00:00");

            mocks.MockDateTimeService
                .Setup(x => x.Now)
                .Returns(DateTime.Parse("10-Jan-2014 10:00:03"));

            List<JenkinsJob> jobsFromWeb = new List<JenkinsJob>()
            {
                new JenkinsJob()
                {
                    Color = "Green",
                    Name = "TestJob3",
                    Url = "http:\\someUrl3"
                }
            };

            mocks.MockJenkinsApi
                .Setup(x => x.GetAllJobs())
                .Returns(jobsFromWeb);

            // Act
            target.GetCruiseServerSnapshot();

            // Assert
            mocks.MockJenkinsApi
                .Verify(x => x.GetAllJobs(),
                Times.Once);
        }
   
    }
}
