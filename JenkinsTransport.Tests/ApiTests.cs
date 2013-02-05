using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class ApiTests
    {
        private Api api;
        private const string TestProjectName = "Testing For All Purpose";

        [SetUp]
        public void SetUp()
        {
            var settings = new Settings()
                               {
                                   Server = "http://build.office.comscore.com",
                                   Username = "cssuser",
                                   Password = "c0msc0r3"
                               };
            api = new Api("http://build.office.comscore.com", settings.AuthorizationInformation);
        }

        [Test]
        public void TestGetAllJobs()
        {
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-all.xml"));
            var list = api.GetAllJobs(xDoc);
            CollectionAssert.IsNotEmpty(list);
            Assert.That(list.Count, Is.EqualTo(76));
        }

        [Test]
        public void TestGetProjectStatusGood()
        {
            var now = DateTime.Now;
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-project.xml"));
            var projectStatus = api.GetProjectStatus(xDoc, null);
            Assert.IsNotNull(projectStatus);
            Assert.That(projectStatus.Activity, Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Success));

            Assert.IsNull(projectStatus.Category);
            Assert.IsNull(projectStatus.BuildStage);

            //StringAssert.AreEqualIgnoringCase(projectStatus.Category, String.Empty);
            //StringAssert.AreEqualIgnoringCase(projectStatus.BuildStage, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.Name, "Direct - INT");
            StringAssert.AreEqualIgnoringCase(projectStatus.Queue, "Direct - INT");
            StringAssert.AreEqualIgnoringCase(projectStatus.CurrentMessage, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.Description, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectStatus.LastBuildLabel, "733");
            StringAssert.AreEqualIgnoringCase(projectStatus.LastSuccessfulBuildLabel, "733");
            StringAssert.AreEqualIgnoringCase(projectStatus.WebURL, "http://build.office.comscore.com/job/Direct%20-%20INT/");

            Assert.That(projectStatus.LastBuildDate, Is.EqualTo(DateTime.Parse("1/23/2013 1:50:49 PM")));
            Assert.That(projectStatus.NextBuildTime, Is.EqualTo(DateTime.MaxValue)); 
            Assert.That(projectStatus.QueuePriority, Is.EqualTo(0));

            CollectionAssert.IsEmpty(projectStatus.Parameters);
        }

        [Test]
        [Explicit] // Need to update the U/P in order to see this disabled project.  cssuser does not have permission
        public void TestGetProjectStatusDisabled()
        {
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-project-disabled.xml"));
            var projectStatus = api.GetProjectStatus(xDoc, null);
            Assert.IsNotNull(projectStatus);
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Stopped));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Unknown));
            Assert.That(projectStatus.Activity, Is.EqualTo(ProjectActivity.Sleeping));
            StringAssert.AreEqualIgnoringCase(projectStatus.Name, "MyMetrix API - Regression");
        }

        [Test]
        public void TestGetProjectSnapshot()
        {
            var now = DateTime.Now;
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-project.xml"));
            var projectSnapshot = api.GetProjectStatusSnapshot(xDoc);

            Assert.IsNotNull(projectSnapshot);
            Assert.That(projectSnapshot.Status, Is.EqualTo(ItemBuildStatus.CompletedSuccess));
            StringAssert.AreEqualIgnoringCase(projectSnapshot.Name, "Direct - INT");
            Assert.That(projectSnapshot.TimeOfSnapshot, Is.GreaterThanOrEqualTo(now));
            StringAssert.AreEqualIgnoringCase(projectSnapshot.Description, String.Empty);
            StringAssert.AreEqualIgnoringCase(projectSnapshot.Error, String.Empty);
            Assert.That(projectSnapshot.TimeCompleted, Is.EqualTo(DateTime.Parse("1/23/2013 1:50:49 PM")));
            Assert.IsNull(projectSnapshot.TimeStarted);
            Assert.IsNull(projectSnapshot.TimeOfEstimatedCompletion);
        }

        [Test]
        [Explicit]
        public void TestForceAndAbortBuild()
        {
            api.ForceBuild(TestProjectName);

            Thread.Sleep(6000); // Sleep for 3 seconds to give the server time

            // Get the latest status of this project
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/" + TestProjectName, null);
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Success));
            Assert.IsTrue(projectStatus.Activity.IsBuilding());

            api.AbortBuild(TestProjectName);

            Thread.Sleep(3000); // Sleep for 3 seconds to give the server time
            projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/" + TestProjectName, null);
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Cancelled).Or.EqualTo(IntegrationStatus.Success)); // This may happen too fast
            Assert.IsFalse(projectStatus.Activity.IsBuilding());
        }

        [Test]
        [Explicit]
        public void TestStopProject()
        {
            api.StopProject(TestProjectName);

            Thread.Sleep(3000); // Sleep for 3 seconds to give the server time
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/" + TestProjectName, null);
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Stopped));
        }

        [Test]
        [Explicit]
        public void TestStartProject()
        {
            api.StartProject(TestProjectName);
            Thread.Sleep(3000); // Sleep for 3 seconds to give the server time

            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/" + TestProjectName, null);
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
        }
    } 
}
