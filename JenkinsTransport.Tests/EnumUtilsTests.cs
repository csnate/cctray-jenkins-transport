using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.Tests
{
    [TestFixture]
    public class EnumUtilsTests
    {
        [Test]
        public void TestGetIntegrationStatus()
        {
            Assert.That(EnumUtils.GetIntegrationStatus("blue"), Is.EqualTo(IntegrationStatus.Success));
            Assert.That(EnumUtils.GetIntegrationStatus("yellow"), Is.EqualTo(IntegrationStatus.Exception));
            Assert.That(EnumUtils.GetIntegrationStatus("red"), Is.EqualTo(IntegrationStatus.Failure));
            Assert.That(EnumUtils.GetIntegrationStatus("grey"), Is.EqualTo(IntegrationStatus.Unknown));
            Assert.That(EnumUtils.GetIntegrationStatus("disabled"), Is.EqualTo(IntegrationStatus.Unknown));
            Assert.That(EnumUtils.GetIntegrationStatus("blue_anime"), Is.EqualTo(IntegrationStatus.Success));
            Assert.That(EnumUtils.GetIntegrationStatus("yellow_anime"), Is.EqualTo(IntegrationStatus.Exception));
            Assert.That(EnumUtils.GetIntegrationStatus("red_anime"), Is.EqualTo(IntegrationStatus.Failure));
            Assert.That(EnumUtils.GetIntegrationStatus("grey_anime"), Is.EqualTo(IntegrationStatus.Unknown));
            Assert.That(EnumUtils.GetIntegrationStatus("disabled_anime"), Is.EqualTo(IntegrationStatus.Unknown));
        }

        [Test]
        public void TestGetProjectActivity()
        {
            Assert.That(EnumUtils.GetProjectActivity("blue"), Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(EnumUtils.GetProjectActivity("yellow"), Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(EnumUtils.GetProjectActivity("red"), Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(EnumUtils.GetProjectActivity("grey"), Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(EnumUtils.GetProjectActivity("disabled"), Is.EqualTo(ProjectActivity.Sleeping));
            Assert.That(EnumUtils.GetProjectActivity("blue_anime"), Is.EqualTo(ProjectActivity.Building));
            Assert.That(EnumUtils.GetProjectActivity("yellow_anime"), Is.EqualTo(ProjectActivity.Building));
            Assert.That(EnumUtils.GetProjectActivity("red_anime"), Is.EqualTo(ProjectActivity.Building));
            Assert.That(EnumUtils.GetProjectActivity("grey_anime"), Is.EqualTo(ProjectActivity.Building));
            Assert.That(EnumUtils.GetProjectActivity("disabled_anime"), Is.EqualTo(ProjectActivity.Building));
        }

        [Test]
        public void TestGetProjectIntegratorStateByColor()
        {
            Assert.That(EnumUtils.GetProjectIntegratorState("blue"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("yellow"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("red"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("grey"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("disabled"), Is.EqualTo(ProjectIntegratorState.Stopped));
            Assert.That(EnumUtils.GetProjectIntegratorState("blue_anime"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("yellow_anime"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("red_anime"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("grey_anime"), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState("disabled_anime"), Is.EqualTo(ProjectIntegratorState.Running));
        }

        [Test]
        public void TestGetProjectIntegratorStateByBuildable()
        {
            Assert.That(EnumUtils.GetProjectIntegratorState(true), Is.EqualTo(ProjectIntegratorState.Running));
            Assert.That(EnumUtils.GetProjectIntegratorState(false), Is.EqualTo(ProjectIntegratorState.Stopped));
        }
    }
}
