using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsServerManagerTests
    {
        protected JenkinsServerManager Manager;

        [SetUp]
        public void SetUp()
        {
            Manager = new JenkinsServerManager();
            Manager.SetConfiguration(new BuildServer("http://build.office.comscore.com"));
            Manager.Settings = new Settings()
                                   {
                                       Username = "cssuser",
                                       Password = "c0msc0r3",
                                       Server = "http://build.office.comscore.com"
                                   };
        }

        [Test]
        public void TestInstantiation()
        {
            Assert.IsInstanceOf<JenkinsServerManager>(Manager);
            StringAssert.AreEqualIgnoringCase(Manager.DisplayName, "http://build.office.comscore.com");
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
        public void TestGetProjectList()
        {
            var list = Manager.GetProjectList();
            CollectionAssert.IsNotEmpty(list);

            foreach (var ccTrayProject in list)
            {
                Console.WriteLine(ccTrayProject.ProjectName);
            }
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
