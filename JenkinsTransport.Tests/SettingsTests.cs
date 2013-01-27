using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class SettingsTests
    {
        [Test]
        public void TestSerialization()
        {
            var settings = new Settings()
                               {
                                   Project = "Test Project",
                                   Server = "Test Server",
                                   Username = "Tester",
                                   Password = "TestPassword"
                               };
            var serializedString = settings.ToString().Replace(Environment.NewLine, String.Empty);
            StringAssert.IsMatch("<JenkinsServerManagerSettings.*?>.*?</JenkinsServerManagerSettings>", serializedString);
            StringAssert.IsMatch("<Server.*?>Test Server</Server>", serializedString);
            StringAssert.IsMatch("<Project.*?>Test Project</Project>", serializedString);
            StringAssert.IsMatch("<Username.*?>Tester</Username>", serializedString);
            StringAssert.IsMatch("<Password.*?>TestPassword</Password>", serializedString);
        }

        [Test]
        public void TestDeseralization()
        {
            var settings =
                Settings.GetSettings(
                    "<JenkinsServerManagerSettings><Server>Test Server</Server><Project>Test Project</Project><Username>Tester</Username><Password>TestPassword</Password></JenkinsServerManagerSettings>");
            StringAssert.AreEqualIgnoringCase(settings.Project, "Test Project");
            StringAssert.AreEqualIgnoringCase(settings.Server, "Test Server");
            StringAssert.AreEqualIgnoringCase(settings.Username, "Tester");
            StringAssert.AreEqualIgnoringCase(settings.Password, "TestPassword");
        }
    }
}
