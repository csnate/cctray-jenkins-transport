using FluentAssertions;
using JenkinsTransport.Interface;
using JenkinsTransport.UnitTests.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
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

        public void EnqueueThisDocumentAsNextResponse(XDocument document)
        {
            var webResponse = new TestWebResponse(document);

            _responses.Enqueue(webResponse);    
        }
    }

    [TestClass]
    public class ApiTests
    {
        protected string ProjectUrl = "https://builds.apache.org/job/Hadoop-1-win/api/xml";
        protected string ProjectName = "Hadoop-1-win";

        [ClassInitialize]
        public static void TestFixtureSetup(TestContext testContext)
        {
            EmbeddedResourceHelper.ExtractManifestResourceToDisk(
                "TestData.BuildInformationSampleData1.xml", 
                ".\\TestData\\BuildInformationSampleData1.xml");

            EmbeddedResourceHelper.ExtractManifestResourceToDisk(
                "TestData.BuildParametersSampleData1.xml",
                ".\\TestData\\BuildParametersSampleData1.xml");

            EmbeddedResourceHelper.ExtractManifestResourceToDisk(
                "TestData.ProjectStatusSampleData1.xml",
                ".\\TestData\\ProjectStatusSampleData1.xml");

            EmbeddedResourceHelper.ExtractManifestResourceToDisk(
               "TestData.ProjectStatusSampleData2_LastSuccessfulBuild.xml",
               ".\\TestData\\ProjectStatusSampleData2_LastSuccessfulBuild.xml");

            EmbeddedResourceHelper.ExtractManifestResourceToDisk(
               "TestData.TestJobsSampleData1.xml",
               ".\\TestData\\TestJobsSampleData1.xml");
        }

        private Api CreateTestTarget(ApiTestDependencies dependencies)
        {
            var target = new Api("https://builds.apache.org/", String.Empty, dependencies.WebRequestFactory);

            return target;
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
        public void GetProjectStatus_when_build_number_has_not_changed_should_return_current_status()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(100);

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            // Act
            ProjectStatus status = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            status.Should().BeSameAs(currentStatus);
        }

        [TestMethod]
        public void GetProjectStatus_when_current_status_is_null_should_return_new_status()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildInformationSampleData1.xml");

            // Act
            ProjectStatus status = target.GetProjectStatus(projectStatusSampleData.Document, null);

            // Assert
            status.Should().NotBeNull();
        }

        [TestMethod]
        public void GetProjectStatus_when_build_number_has_changed_should_return_new_status()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            mocks.EnqueueThisFileAsNextResponse(@".\TestData\BuildInformationSampleData1.xml");

            // Act
            ProjectStatus status = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            status.Should().NotBeSameAs(currentStatus);
        }

        [TestMethod]
        public void GetProjectStatus_when_new_status_should_have_correct_build_number()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            var buildInformationSampleData =
                new BuildInformationSampleData();
            buildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            buildInformationSampleData.SetBuildNumberTo(101);

            mocks.EnqueueThisDocumentAsNextResponse(buildInformationSampleData.Document);

            // Act
            ProjectStatus status = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            status.LastBuildLabel.Should().Be("101");
        }

        [TestMethod]
        public void GetProjectStatus_when_valid_last_completed_build_should_get_new_status_with_it()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            var buildInformationSampleData =
                new BuildInformationSampleData();
            buildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            buildInformationSampleData.SetBuildNumberTo(101);
  
            mocks.EnqueueThisDocumentAsNextResponse(buildInformationSampleData.Document);

            // Act
            var actual = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            actual.LastBuildLabel.Should().Be("101");            
        }

        [TestMethod]
        public void GetProjectStatus_when_no_valid_last_successful_build_should_use_new_build_information()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);
            projectStatusSampleData.SetLastCompletedBuildUrlTo(@"http://testurl");

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            var buildInformationSampleData =
                new BuildInformationSampleData();
            buildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            buildInformationSampleData.SetBuildNumberTo(101);

            mocks.EnqueueThisDocumentAsNextResponse(buildInformationSampleData.Document);

            // Act
            var actual = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            actual.LastSuccessfulBuildLabel.Should().Be("");
        }

        [TestMethod]
        public void GetProjectStatus_when_last_successful_build_exists_and_is_different_from_last_completed_build_number_should_use_last_successful_build_number()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData2_LastSuccessfulBuild.xml");
            projectStatusSampleData.SetLastSuccessfulBuildNumberTo(65835);
            
            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "65833"
                };

            // Configure the first request to GetBuildInformation which will be for the lastCompletedBuild
            // to match the last successful build number
            var buildInformationSampleData =
                new BuildInformationSampleData();
            buildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            buildInformationSampleData.SetBuildNumberTo(65834);

            mocks.EnqueueThisDocumentAsNextResponse(buildInformationSampleData.Document);

            // Act
            var actual = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            actual.LastSuccessfulBuildLabel.Should().Be("65835");
        }

        [TestMethod]
        public void GetProjectStatus_when_last_successful_build_exists_and_is_the_same_as_last_completed_build_number_should_use_last_successful_build_number()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData2_LastSuccessfulBuild.xml");
            projectStatusSampleData.SetLastSuccessfulBuildNumberTo(65834);

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "65833"
                };

            // Configure the first request to GetBuildInformation which will be for the lastCompletedBuild
            // to match the last successful build number
            var lastCompletedBuildInformationSampleData =
                new BuildInformationSampleData();
            lastCompletedBuildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            lastCompletedBuildInformationSampleData.SetBuildNumberTo(65834);

            mocks.EnqueueThisDocumentAsNextResponse(lastCompletedBuildInformationSampleData.Document);

            // Act
            var actual = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            actual.LastSuccessfulBuildLabel.Should().Be("65834");
        }

        [TestMethod]
        public void GetProjectStatus_when_no_valid_last_completed_build_should_use_new_build_information()
        {
            ApiTestDependencies mocks = new ApiTestDependencies();
            var target = CreateTestTarget(mocks);

            ProjectStatusSampleData projectStatusSampleData =
                new ProjectStatusSampleData();

            projectStatusSampleData.InitializeFromFile(@".\TestData\ProjectStatusSampleData1.xml");
            projectStatusSampleData.SetLastBuildNumberTo(101);
            projectStatusSampleData.RemoveLastCompletedBuildElements();

            ProjectStatus currentStatus =
                new ProjectStatus()
                {
                    LastBuildLabel = "100"
                };

            var buildInformationSampleData =
                new BuildInformationSampleData();
            buildInformationSampleData.InitializeFromFile(@".\TestData\BuildInformationSampleData1.xml");
            buildInformationSampleData.SetBuildNumberTo(101);

            mocks.EnqueueThisDocumentAsNextResponse(buildInformationSampleData.Document);

            // Act
            var actual = target.GetProjectStatus(projectStatusSampleData.Document, currentStatus);

            // Assert
            actual.LastBuildLabel.Should().Be(""); 
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
