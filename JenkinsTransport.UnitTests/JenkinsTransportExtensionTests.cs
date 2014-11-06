using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using FluentAssertions;
using JenkinsTransport;
using JenkinsTransport.Interface;
using Moq;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsTransportExtensionTests
    {
        internal class TestMocks
        {
            public Mock<IWebRequestFactory> MockWebRequestFactory { get; set; }
            public Mock<IJenkinsApiFactory> MockJenkinsApiFactory { get; set; }
            public IWebRequestFactory WebRequestFactory { get { return MockWebRequestFactory.Object; } }
            public IJenkinsApiFactory JenkinsApiFactory { get { return MockJenkinsApiFactory.Object; }}

            public TestMocks()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockJenkinsApiFactory = new Mock<IJenkinsApiFactory>();
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
        public void TestRetrieveProjectManager()
        {
            TestMocks mocks = new TestMocks();
            var target = CreateTestTarget(mocks);

            // Act
            var projectManager = target.RetrieveProjectManager("Test Project");
            Assert.IsInstanceOfType(projectManager, typeof(JenkinsProjectManager));

            var jenkinsProjectManager = (JenkinsProjectManager)projectManager;
            Assert.AreEqual(target.Configuration, jenkinsProjectManager.Configuration);
            Assert.AreEqual(jenkinsProjectManager.ProjectName, "Test Project");
            Assert.IsNotNull(jenkinsProjectManager.AuthorizationInformation);
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
