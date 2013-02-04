﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

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
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-all.xml"));
            var list = api.GetAllJobs(xDoc);
            CollectionAssert.IsNotEmpty(list);
            Assert.That(list.Count, Is.EqualTo(40));
        }

        [Test]
        public void TestGetProjectStatusGood()
        {
            var now = DateTime.Now;
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-project.xml"));
            var projectStatus = api.GetProjectStatus(xDoc);
            Assert.IsNotNull(projectStatus);
            Assert.That(projectStatus.Activity, Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Success));

            StringAssert.AreEqualIgnoringCase(projectStatus.Category, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.Name, "Direct - INT");
            StringAssert.AreEqualIgnoringCase(projectStatus.Queue, "Direct - INT");
            StringAssert.AreEqualIgnoringCase(projectStatus.CurrentMessage, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.Description, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.BuildStage, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.LastBuildLabel, "733");
            StringAssert.AreEqualIgnoringCase(projectStatus.LastSuccessfulBuildLabel, "733");
            StringAssert.AreEqualIgnoringCase(projectStatus.WebURL, "http://build.office.comscore.com/job/Direct%20-%20INT/");

            Assert.That(projectStatus.LastBuildDate, Is.EqualTo(DateTime.Parse("1/23/2013 1:50:49 PM")));
            Assert.That(projectStatus.NextBuildTime, Is.GreaterThanOrEqualTo(now));
            Assert.That(projectStatus.QueuePriority, Is.EqualTo(0));

            CollectionAssert.IsEmpty(projectStatus.Parameters);
        }

        [Test]
        public void TestGetProjectStatusDisabled()
        {
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/MyMetrix%20API%20-%20Regression/");
            Assert.IsNotNull(projectStatus);
        }
    }
}
