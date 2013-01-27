using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsProjectManagerTests
    {
        protected ICruiseProjectManager Manager;

        [SetUp]
        public void SetUp()
        {
            Manager = new JenkinsProjectManager();
        }

        [Test]
        public void TestInstantiation()
        {
            Assert.IsInstanceOf<JenkinsProjectManager>(Manager);
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
        [ExpectedException]
        public void TestRetrieveSnapshot()
        {
            var snapshot = Manager.RetrieveSnapshot();
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
