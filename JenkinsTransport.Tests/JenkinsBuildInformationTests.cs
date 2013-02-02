using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class JenkinsBuildInformationTests
    {
        [Test]
        public void TestConstructor()
        {
            //var pathToXml = Path.Combine(Assembly.GetAssembly(typeof(JenkinsBuildInformation)).Location,
            //                             "../../../Examples/jenkins-api-project.xml");
            var xDoc = XDocument.Parse(File.ReadAllText("../../../Examples/jenkins-api-build-info.xml"));
            var info = new JenkinsBuildInformation(xDoc);
            var d = new DateTime(1970, 1, 1).ToLocalTime().AddMilliseconds(1358970649000);

            Assert.IsNotNull(info);
            Assert.That(info.Id, Is.EqualTo("2013-01-23_14-50-49"));
            Assert.That(info.Number, Is.EqualTo("733"));
            Assert.That(info.Duration, Is.EqualTo(964519));
            Assert.That(info.EstimatedDuration, Is.EqualTo(878719));
            Assert.That(info.FullDisplayName, Is.EqualTo("Direct - INT #733"));
            Assert.That(info.Timestamp, Is.EqualTo(d));

            Console.WriteLine(d.ToString());
        }
    }
}
