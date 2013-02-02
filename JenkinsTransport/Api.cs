using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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
            return GetAllJobs(xDoc);
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        public List<JenkinsJob> GetAllJobs(XDocument xDoc)
        {
            var list = xDoc.Descendants("job").Select(a => new JenkinsJob(a)).ToList();
            return list;
        } 

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="projectUrl">the project url to retrieve the info</param>
        public ProjectStatus GetProjectStatus(string projectUrl)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(projectUrl + ProjectStatus, AuthInfo);
            return GetProjectStatus(xDoc);
        }

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        public ProjectStatus GetProjectStatus(XDocument xDoc)
        {
            var color = (string)xDoc.Element("color");
            return new ProjectStatus(
                (string)xDoc.Element("name"),
                String.Empty, // Category
                EnumUtils.GetProjectActivity(color),
                EnumUtils.GetIntegrationStatus(color),
                EnumUtils.GetProjectIntegratorState((bool)xDoc.Element("buildable")),
                projectUrl, // webUrl
                (DateTime)xDoc.Element(String.Empty), // LastBuildDate
                (string)xDoc.Element(String.Empty), // LastBuildLabel
                (string)xDoc.Element(String.Empty), // LastSuccessfulBuildLabel
                (DateTime)xDoc.Element(String.Empty), // NextBuildTime
                (string)xDoc.Element(String.Empty), // BuildStage
                String.Empty, // Queue - not used
                0, // QueuePriority - not used
                new List<ParameterBase>() // Parameters - not used yet
                );
        }

        /// <summary>
        /// Get the build information for a build information url
        /// </summary>
        /// <param name="buildInformationUrl">the build information url</param>
        public JenkinsBuildInformation GetBuildInformation(string buildInformationUrl)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(buildInformationUrl + ProjectStatus, AuthInfo);
            return new JenkinsBuildInformation(xDoc);
        }
    }
}
