using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class Api
    {
        #region Constants
        private const string XmlApi = "/api/xml";
        private const string AllJobs = XmlApi + "?xpath=/hudson/job&wrapper=jobs";
        private const string ExcludeBuild = XmlApi + "?exclude=freeStyleProject/build";
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
        /// <param name="currentStatus">the current stored status</param>
        public ProjectStatus GetProjectStatus(string projectUrl, ProjectStatus currentStatus)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(projectUrl + ExcludeBuild, AuthInfo);
            return GetProjectStatus(xDoc, currentStatus);
        }

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        /// <param name="currentStatus">the current stored status</param>
        public ProjectStatus GetProjectStatus(XDocument xDoc, ProjectStatus currentStatus)
        {
            var firstElement = xDoc.Element("freeStyleProject");
            var color = (string)firstElement.Element("color");
            var lastBuildElement = firstElement.Element("lastBuild");  // Will contain the latest (in progress) build number
            var lastSuccessfulBuildElement = firstElement.Element("lastSuccessfulBuild");
            var lastCompletedBuild = firstElement.Element("lastCompletedBuild");

            // Check if the last build number is any different from the current status.  If not, then update
            if (currentStatus != null && lastBuildElement != null && currentStatus.LastBuildLabel == (string)lastBuildElement.Element("number"))
            {
                return currentStatus;
            }

            // Otherwise, we'll need to get the new status
            var lastCompletedBuildInfo = lastCompletedBuild != null
                                    ? GetBuildInformation((string) lastCompletedBuild.Element("url"))
                                    : new JenkinsBuildInformation();

            // Check to see if the last successfull is the same as the last build.  If so, no need to get the details again
            var lastSuccessfulBuildInfo = lastSuccessfulBuildElement != null
                                              ? lastCompletedBuildInfo.Number == (string) lastSuccessfulBuildElement.Element("number")
                                                    ? lastCompletedBuildInfo
                                                    : GetBuildInformation((string) lastSuccessfulBuildElement.Element("url"))
                                              : new JenkinsBuildInformation();

            var name = (string) firstElement.Element("name");
            return new ProjectStatus(
                name,
                String.Empty, // Category
                EnumUtils.GetProjectActivity(color),
                EnumUtils.GetIntegrationStatus(color),
                EnumUtils.GetProjectIntegratorState((bool) firstElement.Element("buildable")),
                (string) firstElement.Element("url"), // webUrl
                lastCompletedBuildInfo.Timestamp, // LastBuildDate
                lastCompletedBuildInfo.Number, // LastBuildLabel
                lastSuccessfulBuildInfo.Number, // LastSuccessfulBuildLabel
                DateTime.Now, // NextBuildTime -- TODO - this is incorrect, but I don't know how to get the next build time
                String.Empty, // BuildStage
                name, // Queue - not used
                0, // QueuePriority - not used
                new List<ParameterBase>() // Parameters - not used yet
                )
                       {
                           Description = (string) firstElement.Element("description"),
                           ShowForceBuildButton = false, // Disable this for now
                           ShowStartStopButton = false // Disable this for now
                       };
        }

        /// <summary>
        /// Get the build information for a build information url
        /// </summary>
        /// <param name="buildInformationUrl">the build information url, without /api/xml</param>
        public JenkinsBuildInformation GetBuildInformation(string buildInformationUrl)
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(buildInformationUrl + XmlApi, AuthInfo);
            return new JenkinsBuildInformation(xDoc);
        }

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="projectName">the project name to check</param>
        public ProjectStatusSnapshot GetProjectStatusSnapshot(string projectName)
        {
            var url = BaseUrl + "/job/" + HttpUtility.HtmlEncode(projectName) + ExcludeBuild;
            var xDoc = XmlUtils.GetXDocumentFromUrl(url, AuthInfo);
            return GetProjectStatusSnapshot(xDoc);
        }

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="xDoc">the XDcoument to parse</param>
        public ProjectStatusSnapshot GetProjectStatusSnapshot(XDocument xDoc)
        {
            var firstElement = xDoc.Element("freeStyleProject");
            var color = (string)firstElement.Element("color");
            var lastBuildInfo = GetBuildInformation((string) firstElement.Element("lastBuild").Element("url"));
            var status = lastBuildInfo.Building ? ItemBuildStatus.Running : EnumUtils.GetItemBuildStatus(color);

            var snapshot = new ProjectStatusSnapshot
                               {
                                   Status = status,
                                   Name = (string) firstElement.Element("name"),
                                   TimeOfSnapshot = DateTime.Now,
                                   Description = (string) firstElement.Element("description"),
                                   Error = String.Empty // Not sure what to do with this yet
                               };

            // Set one or the other
            if (status == ItemBuildStatus.Running)
            {
                snapshot.TimeStarted = lastBuildInfo.Timestamp;
                snapshot.TimeOfEstimatedCompletion =
                    lastBuildInfo.Timestamp.AddMilliseconds(lastBuildInfo.EstimatedDuration);
            }
            else
            {
                snapshot.TimeCompleted = lastBuildInfo.Timestamp;
            }

            return snapshot;
        }
    }
}
