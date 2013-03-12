using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
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
            StringAssert.Matches(serializedString, new Regex("<JenkinsServerManagerSettings.*?>.*?</JenkinsServerManagerSettings>"));
            StringAssert.Matches(serializedString, new Regex("<Server.*?>" + settings.Server + "</Server>"));
            StringAssert.Matches(serializedString, new Regex("<Project.*?>" + settings.Project + "</Project>"));
            StringAssert.Matches(serializedString, new Regex("<AuthorizationInformation.*?>" +  settings.AuthorizationInformation + "</AuthorizationInformation>"));
            StringAssert.DoesNotMatch(serializedString, new Regex("<Username.*?>" + settings.Username + "</Username>"));
            StringAssert.DoesNotMatch(serializedString, new Regex("<Password.*?>" + settings.Password + "</Password>"));
        }

        [TestMethod]
        public void TestDeseralization()
        {
            var settings =
                Settings.GetSettings(
                    "<JenkinsServerManagerSettings><Server>Test Server</Server><Project>Test Project</Project><AuthorizationInformation>TestAuthorizationInformation</AuthorizationInformation></JenkinsServerManagerSettings>");
            Assert.AreEqual(settings.Project, "Test Project", true);
            Assert.AreEqual(settings.Server, "Test Server", true);
            Assert.AreEqual(settings.AuthorizationInformation, "TestAuthorizationInformation", true);
            Assert.IsNull(settings.Username);
            Assert.IsNull(settings.Password);
        }
    }
}
