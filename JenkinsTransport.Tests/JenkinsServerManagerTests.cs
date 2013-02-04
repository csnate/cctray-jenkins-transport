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
            Manager.Initialize(new BuildServer("http://build.office.comscore.com"), String.Empty, new Settings()
                                   {
                                       Username = "cssuser",
                                       Password = "c0msc0r3",
                                       Server = "http://build.office.comscore.com"
                                   });
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
        public void TestGetCruiseServerSnapshot()
        {
            var snapshot = Manager.GetCruiseServerSnapshot();
            CollectionAssert.IsNotEmpty(snapshot.ProjectStatuses);
            Assert.That(snapshot.ProjectStatuses.Length, Is.EqualTo(4));
            CollectionAssert.IsEmpty(snapshot.QueueSetSnapshot.Queues);
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
        public void TestLogin()
        {
            Manager.Login();
            Assert.IsNotNull(Manager.AuthorizationInformation);
        }

        [Test]
        public void TestLogout()
        {
            Manager.Logout();
            Assert.IsNullOrEmpty(Manager.AuthorizationInformation);
        }
    }
}
