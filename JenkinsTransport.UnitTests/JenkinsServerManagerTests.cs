﻿using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsServerManagerTests
    {
        protected JenkinsServerManager Manager;

        [TestInitialize]
        public void Setup()
        {
            Manager = new JenkinsServerManager();
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
        public void TestGetProjectList()
        {
            var projectList = Manager.GetProjectList();
            Assert.IsTrue(projectList.Any());
        }

        [TestMethod]
        public void TestGetCruiseServerSnapshot()
        {
            var projectList = Manager.GetProjectList();
            Manager.ProjectsAndCurrentStatus.Add(projectList[0].ProjectName, null);

            var snapshot = Manager.GetCruiseServerSnapshot();
            Assert.IsNotNull(snapshot);
            Assert.IsTrue(snapshot.ProjectStatuses.Any());
            Assert.IsTrue(snapshot.ProjectStatuses.Count() == 1);
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
    }
}
