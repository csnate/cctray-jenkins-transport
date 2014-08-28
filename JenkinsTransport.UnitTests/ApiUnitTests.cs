using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using FluentAssertions;
using JenkinsTransport.Interface;
using JenkinsTransport.UnitTests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThoughtWorks.CruiseControl.Remote;


namespace JenkinsTransport.UnitTests
{
    internal class ApiTestDependencies
    {
        public Mock<IWebRequestFactory> MockWebRequestFactory;
        public IWebRequestFactory WebRequestFactory { get { return MockWebRequestFactory.Object; }}
        private Queue<TestWebResponse> _responses = new Queue<TestWebResponse>(); 

        public ApiTestDependencies()
        {
            MockWebRequestFactory = new Mock<IWebRequestFactory>();

            Mock<IWebRequest> mockWebRequest = new Mock<IWebRequest>();
            mockWebRequest
                .Setup(x => x.GetResponse())
                .Returns(() => _responses.Dequeue());

            MockWebRequestFactory
               .Setup(x => x.Create(It.IsAny<string>()))
               .Returns(mockWebRequest.Object);

        }

        public void EnqueueThisFileAsNextResponse(string sampleData)
        {
            Stream responseStream = new FileStream(sampleData, FileMode.Open);

            var webResponse = new TestWebResponse(responseStream);

            _responses.Enqueue(webResponse);            
        }
    }

    [TestClass]
    public class ApiUnitTests
    {
        protected string ProjectUrl = "https://builds.apache.org/job/Hadoop-1-win/api/xml";
        protected string ProjectName = "Hadoop-1-win";


        private Api CreateTestTarget(ApiTestDependencies dependencies)
        {
            var target = new Api("https://builds.apache.org/", String.Empty, dependencies.WebRequestFactory);

            return target;
        }

        //[TestMethod]
        public void CollectTestData()
        {
            var target = new Api("https://builds.apache.org/", String.Empty, new WebRequestFactory());
            target.GetBuildParameters(ProjectUrl);
            
        }

        [TestMethod]
        public void GetAllJobs_should_return_correct_number()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\TestJobsSampleData1.xml");       

            // Act
            var jobs = target.GetAllJobs();

            // Assert
            jobs.Count.Should().Be(1076);
            CollectionAssert.AllItemsAreUnique(jobs);
        }

        [TestMethod]
        public void GetProjectStatus_should_have_correct_project_name()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\ProjectStatusSampleData1.xml");
            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildInformationSampleData1.xml");

            // Act
            var status = target.GetProjectStatus(ProjectUrl, null);

            // Assert
            status.Name.Should().Be(ProjectName);
        }

        [TestMethod]
        public void GetProjectStatus_should_have_correct_webUrl()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\ProjectStatusSampleData1.xml");
            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildInformationSampleData1.xml");

            // Act
            ProjectStatus status = target.GetProjectStatus(ProjectUrl, null);

            // Assert
            status.WebURL.Should().Be("https://builds.apache.org/job/Hadoop-1-win/");
        }

        [TestMethod]
        public void GetBuildInformation_should_have_correct_webUrl()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildInformationSampleData1.xml");
            // TODO add a response for the build information

            // Act
            JenkinsBuildInformation status = target.GetBuildInformation(ProjectUrl);

            // Assert
            status.FullDisplayName.Should().Be("Hadoop-1-win #119");
        }

        [TestMethod]
        public void GetBuildParameters_should_return_correct_number_of_parameters()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildParametersSampleData1.xml");
            // TODO add a response for the build information

            // Act
            var status = target.GetBuildParameters(ProjectUrl);

            // Assert
            status.Count.Should().Be(1);
            status[0].Name.Should().Be("VERSION");
        }

        [TestMethod]
        public void GetBuildParameters_should_return_correct_name_of_parameters()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildParametersSampleData1.xml");
            // TODO add a response for the build information

            // Act
            var status = target.GetBuildParameters(ProjectUrl);

            // Assert
            status[0].Name.Should().Be("VERSION");
        }
    }
}
