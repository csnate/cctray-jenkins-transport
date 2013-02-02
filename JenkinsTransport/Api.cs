using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class Api
    {
        #region Constants
        private const string AllJobs = "/api/xml?xpath=/hudson/job&wrapper=jobs";
        private const string ProjectStatus = "/api/xml";
        #endregion

        protected string BaseUrl { get; set; }
        protected string AuthInfo { get; set; }

        public Api(string baseUrl, string authInfo)
        {
            BaseUrl = baseUrl;
            AuthInfo = authInfo;
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        public List<JenkinsJob> GetAllJobs()
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(BaseUrl + AllJobs, AuthInfo);
            var list = xDoc.Descendants("job").Select(a => new JenkinsJob(a)).ToList();
            return list;
        }

        public ProjectStatus GetProjectStatus(string projectUrl)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(projectUrl + ProjectStatus, AuthInfo);
            var color = (string) xDoc.Element("color");
            return new ProjectStatus(
                (string) xDoc.Element("name"),
                String.Empty, // Category
                EnumUtils.GetProjectActivity(color),
                EnumUtils.GetIntegrationStatus(color),
                EnumUtils.GetProjectIntegratorState((bool) xDoc.Element("buildable")),
                projectUrl, // webUrl
                (DateTime) xDoc.Element(String.Empty), // LastBuildDate
                (string) xDoc.Element(String.Empty), // LastBuildLabel
                (string) xDoc.Element(String.Empty), // LastSuccessfulBuildLabel
                (DateTime) xDoc.Element(String.Empty), // NextBuildTime
                (string) xDoc.Element(String.Empty), // BuildStage
                String.Empty, // Queue - not used
                0, // QueuePriority - not used
                new List<ParameterBase>() // Parameters - not used yet
                );
        }

        public JenkinsBuildInformation GetBuildInformation(string buildInformationUrl)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(buildInformationUrl + ProjectStatus, AuthInfo);
            return new JenkinsBuildInformation(xDoc);
        }
    }
}
