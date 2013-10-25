using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class JenkinsBuildInformationTests
    {
        private const string BuildInformationXml = @"<freeStyleBuild>
    <action></action>
    <action>
        <cause>
            <shortDescription>Started by an SCM change</shortDescription>
        </cause>
    </action>
    <action></action>
    <action></action>
    <action></action>
    <action>
        <failCount>0</failCount>
        <skipCount>44</skipCount>
        <totalCount>565</totalCount>
        <urlName>testReport</urlName>
    </action>
    <action></action>
    <building>false</building>
    <duration>964519</duration>
    <estimatedDuration>878719</estimatedDuration>
    <fullDisplayName>Test Project #1</fullDisplayName>
    <id>2013-01-23_14-50-49</id>
    <keepLog>false</keepLog>
    <number>1</number>
    <result>SUCCESS</result>
    <timestamp>1358970649000</timestamp>
    <url>http://domain.com/job/Test%20Project/1/</url>
    <builtOn></builtOn>
    <changeSet>
        <item>
            <affectedPath>test.txt</affectedPath>
            <author>
                <absoluteUrl>http://domain.com/user/tester</absoluteUrl>
                <fullName>Tester, Tester</fullName>
            </author>
            <commitId>1</commitId>
            <timestamp>1358970529987</timestamp>
            <date>2013-01-23T19:48:49.987982Z</date>
            <msg>Test commit</msg>
            <path>
                <editType>add</editType>
                <file>test.txt</file>
            </path>
            <revision>5533</revision>
            <user>Tester, Tester</user>
        </item>
        <kind>svn</kind>
        <revision>
            <module>http://domain.com/TestProject/trunk</module>
            <revision>5533</revision>
        </revision>
    </changeSet>
    <culprit>
        <absoluteUrl>http://domain.com/user/tester</absoluteUrl>
        <fullName>Tester, Tester</fullName>
    </culprit>
</freeStyleBuild>";

        private const string BuildInformationInProgressXml = @"<freeStyleBuild>
    <action>
        <cause>
            <shortDescription>Started by an SCM change</shortDescription>
        </cause>
    </action>
    <action></action>
    <building>true</building>
    <duration>0</duration>
    <estimatedDuration>630679</estimatedDuration>
    <executor></executor>
    <fullDisplayName>Test Project #2</fullDisplayName>
    <id>2013-02-04_13-14-21</id>
    <keepLog>false</keepLog>
    <number>841</number>
    <timestamp>1360001661908</timestamp>
    <url>http://domain.com/job/Test%20Project/2/</url>
    <builtOn></builtOn>
    <changeSet></changeSet>
</freeStyleBuild>";

        [TestMethod]
        public void TestConstructorWithFinishedBuild()
        {
            var xDoc = XDocument.Parse(BuildInformationXml);
            var info = new JenkinsBuildInformation(xDoc);
            var d = new DateTime(1970, 1, 1).ToLocalTime().AddMilliseconds(1358970649000);

            Assert.IsNotNull(info);
            Assert.AreEqual(info.Id, "2013-01-23_14-50-49");
            Assert.AreEqual(info.Number, "1");
            Assert.AreEqual(info.Duration, 964519);
            Assert.AreEqual(info.EstimatedDuration, 878719);
            Assert.AreEqual(info.FullDisplayName, "Test Project #1");
            Assert.AreEqual(info.Timestamp, d);
            Assert.IsFalse(info.Building);
        }

        [TestMethod]
        public void TestConstructorWithInProgressBuild()
        {
            var xDoc = XDocument.Parse(BuildInformationInProgressXml);
            var info = new JenkinsBuildInformation(xDoc);

            Assert.IsNotNull(info);
            Assert.IsTrue(info.Building);
        }
    }
}
