using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class EnumUtilsTests
    {
        [TestMethod]
        public void TestGetIntegrationStatus()
        {
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("blue"), IntegrationStatus.Success);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("yellow"), IntegrationStatus.Exception);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("red"), IntegrationStatus.Failure);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("grey"), IntegrationStatus.Unknown);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("aborted"), IntegrationStatus.Failure);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("disabled"), IntegrationStatus.Unknown);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("blue_anime"), IntegrationStatus.Success);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("yellow_anime"), IntegrationStatus.Exception);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("red_anime"), IntegrationStatus.Failure);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("grey_anime"), IntegrationStatus.Unknown);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("aborted_anime"), IntegrationStatus.Failure);
            Assert.AreEqual(EnumUtils.GetIntegrationStatus("disabled_anime"), IntegrationStatus.Unknown);
        }

        [TestMethod]
        public void TestGetProjectActivity()
        {
            Assert.AreEqual(EnumUtils.GetProjectActivity("blue"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("yellow"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("red"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("grey"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("aborted"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("disabled"), ProjectActivity.Sleeping);
            Assert.AreEqual(EnumUtils.GetProjectActivity("blue_anime"), ProjectActivity.Building);
            Assert.AreEqual(EnumUtils.GetProjectActivity("yellow_anime"), ProjectActivity.Building);
            Assert.AreEqual(EnumUtils.GetProjectActivity("red_anime"), ProjectActivity.Building);
            Assert.AreEqual(EnumUtils.GetProjectActivity("grey_anime"), ProjectActivity.Building);
            Assert.AreEqual(EnumUtils.GetProjectActivity("aborted_anime"), ProjectActivity.Building);
            Assert.AreEqual(EnumUtils.GetProjectActivity("disabled_anime"), ProjectActivity.Building);
        }

        [TestMethod]
        public void TestGetProjectIntegratorStateByColor()
        {
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("blue"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("yellow"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("red"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("grey"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("aborted"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("disabled"), ProjectIntegratorState.Stopped);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("blue_anime"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("yellow_anime"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("red_anime"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("grey_anime"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("aborted_anime"), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState("disabled_anime"), ProjectIntegratorState.Running);
        }

        [TestMethod]
        public void TestGetProjectIntegratorStateByBuildable()
        {
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState(true), ProjectIntegratorState.Running);
            Assert.AreEqual(EnumUtils.GetProjectIntegratorState(false), ProjectIntegratorState.Stopped);
        }

        [TestMethod]
        public void TestGetItemBuildStatus()
        {
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("blue"), ItemBuildStatus.CompletedSuccess);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("yellow"), ItemBuildStatus.CompletedFailed);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("red"), ItemBuildStatus.CompletedFailed);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("grey"), ItemBuildStatus.Unknown);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("aborted"), ItemBuildStatus.Cancelled);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("disabled"), ItemBuildStatus.CompletedSuccess);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("blue_anime"), ItemBuildStatus.Running);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("yellow_anime"), ItemBuildStatus.Running);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("red_anime"), ItemBuildStatus.Running);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("grey_anime"), ItemBuildStatus.Running);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("aborted_anime"), ItemBuildStatus.Running);
            Assert.AreEqual(EnumUtils.GetItemBuildStatus("disabled_anime"), ItemBuildStatus.Running);
        }
    }
}
