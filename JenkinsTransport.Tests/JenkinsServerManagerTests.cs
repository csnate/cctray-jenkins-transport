using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsServerManagerTests
    {
        protected ICruiseServerManager Manager;

        [SetUp]
        public void SetUp()
        {
            Manager = new JenkinsServerManager();
        }

        [Test]
        public void TestInstantiation()
        {
            Assert.IsInstanceOf<JenkinsServerManager>(Manager);
            StringAssert.AreEqualIgnoringCase(Manager.DisplayName, String.Empty); // Correct. Only set by TransportExtension.Configure
        }

        [Test]
        [ExpectedException]
        public void TestCancelPendingRequest()
        {
            Manager.CancelPendingRequest(String.Empty);
        }

        [Test]
        [ExpectedException]
        public void TestGetCruiseServerSnapshot()
        {
            var snapshot = Manager.GetCruiseServerSnapshot();
        }

        [Test]
        [ExpectedException]
        public void TestGetProjectList()
        {
            var list = Manager.GetProjectList();
        }

        [Test]
        [ExpectedException]
        public void TestLogin()
        {
            Manager.Login();
        }

        [Test]
        [ExpectedException]
        public void TestLogout()
        {
            Manager.Logout();
        }
    }
}
