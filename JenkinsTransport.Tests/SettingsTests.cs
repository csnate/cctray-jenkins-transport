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
                                   Server = "Test Server"
                               };
            var serializedString = settings.ToString().Replace(Environment.NewLine, String.Empty);
            StringAssert.IsMatch("<JenkinsServerManagerSettings.*?>.*?</JenkinsServerManagerSettings>", serializedString);
            StringAssert.IsMatch("<Server.*?>Test Server</Server>", serializedString);
            StringAssert.IsMatch("<Project.*?>Test Project</Project>", serializedString);
        }

        [Test]
        public void TestDeseralization()
        {
            var settings =
                Settings.GetSettings(
                    "<JenkinsServerManagerSettings><Server>Test Server</Server><Project>Test Project</Project></JenkinsServerManagerSettings>");
            Assert.That(settings.Project, Is.EqualTo("Test Project"));
            Assert.That(settings.Server, Is.EqualTo("Test Server"));
        }
    }
}
