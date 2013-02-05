using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsProjectManagerTests
    {
        protected JenkinsProjectManager Manager;
        private const string TestProjectName = "Testing For All Purpose";

        private ProjectStatus GetLatestProjectStatus()
        {
            var api = new Api("http://build.office.comscore.com", Manager.Settings.AuthorizationInformation);
            var projectStatus = api.GetProjectStatus("http://build.office.comscore.com/job/" + TestProjectName, null);
            return projectStatus;
        }

        [SetUp]
        public void SetUp()
        {
            Manager = new JenkinsProjectManager();
            Manager.Initialize(new BuildServer("http://build.office.comscore.com"), TestProjectName, new Settings()
                                                                                                      {
                                                                                                          Username = "cssuser",
                                                                                                          Password = "c0msc0r3",
                                                                                                          Server = "http://build.office.comscore.com"
                                                                                                      });
        }

        [Test]
        public void TestInstantiation()
        {
            Assert.IsInstanceOf<JenkinsProjectManager>(Manager);
            Assert.That(Manager.ProjectName, Is.EqualTo(TestProjectName));
            Assert.That(Manager.Configuration.Url, Is.EqualTo("http://build.office.comscore.com"));
            Assert.That(Manager.Settings.Server, Is.EqualTo("http://build.office.comscore.com"));
            Assert.That(Manager.Settings.Username, Is.EqualTo("cssuser"));
            Assert.That(Manager.Settings.Password, Is.EqualTo("c0msc0r3"));
            Assert.IsNotNullOrEmpty(Manager.AuthorizationInformation);
        }

        [Test]
        [Explicit]
        public void TestForceBuild()
        {
            // ------------------------
            // This test is a little strange.  I've seen it error out with a 404, however the build is still started
            // I think it's related to the fact that Anonymous needs to have the appropriate access
            // ------------------------

            Manager.ForceBuild(String.Empty, null, String.Empty);

            // Check the latest status
            Thread.Sleep(6000); // This doesn't always work well
            var projectStatus = GetLatestProjectStatus();

            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(projectStatus.BuildStatus, Is.EqualTo(IntegrationStatus.Success));
            Assert.IsTrue(projectStatus.Activity.IsBuilding());
        }

        [Test]
        [ExpectedException]
        public void TestFixBuild()
        {
            Manager.FixBuild(String.Empty, String.Empty);
        }

        [Test]
        [Explicit]
        public void TestAbortBuild()
        {
            // ------------------------
            // This test is difficult to complete successfully.
            // You have to have a build going, and then run this test.
            // ------------------------

            Manager.AbortBuild(String.Empty, String.Empty);

            Thread.Sleep(1000);
            var projectStatus = GetLatestProjectStatus();
            Assert.IsFalse(projectStatus.Activity.IsBuilding());
        }

        [Test]
        [Explicit]
        public void TestStopProject()
        {
            // ------------------------
            // Anonymous MUST have read/build/configure access to the project
            // ------------------------
            Manager.StopProject(String.Empty);

            Thread.Sleep(1000);
            var projectStatus = GetLatestProjectStatus();
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Stopped));
        }

        [Test]
        [Explicit]
        public void TestStartProject()
        {
            // ------------------------
            // Anonymous MUST have read/build/configure access to the project
            // ------------------------
            Manager.StartProject(String.Empty);

            Thread.Sleep(1000);
            var projectStatus = GetLatestProjectStatus();
            Assert.That(projectStatus.Status, Is.EqualTo(ProjectIntegratorState.Running));
        }

        [Test]
        [ExpectedException]
        public void TestCancelPendingRequest()
        {
            Manager.CancelPendingRequest(String.Empty);
        }

        [Test]
        public void TestRetrieveSnapshot()
        {
            var now = DateTime.Now;
            var snapshot = Manager.RetrieveSnapshot();
            Assert.IsNotNull(snapshot);
            StringAssert.AreEqualIgnoringCase(snapshot.Name, TestProjectName);
            Assert.That(snapshot.TimeOfSnapshot, Is.GreaterThanOrEqualTo(now));
            StringAssert.AreEqualIgnoringCase(snapshot.Description, "For test various msbuild script");
            StringAssert.AreEqualIgnoringCase(snapshot.Error, String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestRetrievePackageList()
        {
            var list = Manager.RetrievePackageList();
        }

        [Test]
        [ExpectedException]
        public void TestRetrieveFileTransfer()
        {
            var transfer = Manager.RetrieveFileTransfer(String.Empty);
        }

        [Test]
        public void TestListBuildParameters()
        {
            var list = Manager.ListBuildParameters();
            CollectionAssert.IsEmpty(list); // For now, this should always return an empty list until I implement parameterized builds
        }

    }
}
