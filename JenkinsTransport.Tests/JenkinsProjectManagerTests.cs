using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        [SetUp]
        public void SetUp()
        {
            Manager = new JenkinsProjectManager();
            Manager.Initialize(new BuildServer("http://build.office.comscore.com"), "Direct - INT", new Settings()
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
            Assert.That(Manager.ProjectName, Is.EqualTo("Direct - INT"));
            Assert.That(Manager.Configuration.Url, Is.EqualTo("http://build.office.comscore.com"));
            Assert.That(Manager.Settings.Server, Is.EqualTo("http://build.office.comscore.com"));
            Assert.That(Manager.Settings.Username, Is.EqualTo("cssuser"));
            Assert.That(Manager.Settings.Password, Is.EqualTo("c0msc0r3"));
            Assert.IsNotNullOrEmpty(Manager.AuthorizationInformation);
        }

        [Test]
        [ExpectedException]
        public void TestForceBuild()
        {
            Manager.ForceBuild(String.Empty, null, String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestFixBuild()
        {
            Manager.FixBuild(String.Empty, String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestAbortBuild()
        {
            Manager.AbortBuild(String.Empty, String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestStopProject()
        {
            Manager.StopProject(String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestStartProject()
        {
            Manager.StartProject(String.Empty);
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
            StringAssert.AreEqualIgnoringCase(snapshot.Name, "Direct - INT");
            Assert.That(snapshot.TimeOfSnapshot, Is.GreaterThanOrEqualTo(now));
            StringAssert.AreEqualIgnoringCase(snapshot.Description, String.Empty);
            StringAssert.AreEqualIgnoringCase(snapshot.Error, String.Empty);

            //Assert.That(projectSnapshot.TimeCompleted, Is.EqualTo(DateTime.Parse("1/23/2013 1:50:49 PM")));
            //Assert.IsNull(projectSnapshot.TimeStarted);
            //Assert.IsNull(projectSnapshot.TimeOfEstimatedCompletion);
            //Assert.That(snapshot.Status, Is.EqualTo(ItemBuildStatus.CompletedSuccess));
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
        [ExpectedException]
        public void TestListBuildParameters()
        {
            var list = Manager.ListBuildParameters();
        }

    }
}
