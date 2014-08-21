using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using JenkinsTransport.Interface;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport
{
    public class ApiNonStatic : Api
    {
        public ApiNonStatic(string baseUrl, string authInfo, IWebRequestFactory webRequestFactory)
            : base(baseUrl, authInfo, webRequestFactory)
        {
        }

        protected new XDocument GetXDocument(string url, string authInfo)
        {
            var request = WebRequestFactory.Create(url);

            if (!String.IsNullOrEmpty(authInfo))
            {
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
            request.Method = "GET";

            return GetXDocument(request);
        }

        protected new XDocument GetXDocument(IWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var rd = new StreamReader(responseStream))
                        {
                            return XDocument.Parse(rd.ReadToEnd());
                        }
                    }
                }
            }
            return new XDocument();
        }

        public XDocument GetAllJobsDoc()
        {
            return GetXDocument(BaseUrl + AllJobs, AuthInfo);            
        }

        public XDocument GetProjectStatusDoc(string projectUrl)
        {
            return GetXDocument(projectUrl + ExcludeBuild, AuthInfo);
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        public new List<JenkinsJob> GetAllJobs()
        {
            var xDoc = GetXDocument(BaseUrl + AllJobs, AuthInfo);
            return GetAllJobs(xDoc);
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        public new List<JenkinsJob> GetAllJobs(XDocument xDoc)
        {
            var list = xDoc.Descendants("job").Select(a => new JenkinsJob(a)).ToList();
            return list;
        }

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="projectUrl">the project url to retrieve the info</param>
        /// <param name="currentStatus">the current stored status</param>
        //public new ProjectStatus GetProjectStatus(string projectUrl, ProjectStatus currentStatus)
        //{
        //    var xDoc = GetXDocument(projectUrl + ExcludeBuild, AuthInfo);
        //    return GetProjectStatus(xDoc, currentStatus);
        //}

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        /// <param name="currentStatus">the current stored status</param>
        //public new ProjectStatus GetProjectStatus(XDocument xDoc, ProjectStatus currentStatus)
        //{
        //    // The first element in the document is named based on the type of project (freeStyleProject, mavenModuleSet, etc). Can't access based on name.
        //    var firstElement = xDoc.Descendants().First<XElement>();
        //    var color = (string)firstElement.Element("color");
        //    var lastBuildElement = firstElement.Element("lastBuild");  // Will contain the latest (in progress) build number
        //    var lastSuccessfulBuildElement = firstElement.Element("lastSuccessfulBuild");
        //    var lastCompletedBuild = firstElement.Element("lastCompletedBuild");

        //    // Check if the last build number is any different from the current status.  If not, then update
        //    if (currentStatus != null && lastBuildElement != null && currentStatus.LastBuildLabel == (string)lastBuildElement.Element("number"))
        //    {
        //        return currentStatus;
        //    }

        //    // Otherwise, we'll need to get the new status
        //    var lastCompletedBuildInfo = lastCompletedBuild != null
        //                            ? GetBuildInformation((string)lastCompletedBuild.Element("url"))
        //                            : new JenkinsBuildInformation();

        //    // Check to see if the last successfull is the same as the last build.  If so, no need to get the details again
        //    var lastSuccessfulBuildInfo = lastSuccessfulBuildElement != null
        //                                      ? lastCompletedBuildInfo.Number == (string)lastSuccessfulBuildElement.Element("number")
        //                                            ? lastCompletedBuildInfo
        //                                            : GetBuildInformation((string)lastSuccessfulBuildElement.Element("url"))
        //                                      : new JenkinsBuildInformation();

        //    var name = (string)firstElement.Element("name");
        //    var projectStatus = new ProjectStatus(name, EnumUtils.GetIntegrationStatus(color),
        //                                          lastCompletedBuildInfo.Timestamp)
        //    {
        //        Activity = EnumUtils.GetProjectActivity(color),
        //        Status = EnumUtils.GetProjectIntegratorState((bool)firstElement.Element("buildable")),
        //        WebURL = (string)firstElement.Element("url"),
        //        LastBuildLabel = lastCompletedBuildInfo.Number,
        //        LastSuccessfulBuildLabel = lastSuccessfulBuildInfo.Number,
        //        Queue = name,
        //        QueuePriority = 0,
        //        Description = (string)firstElement.Element("description"),
        //        ShowForceBuildButton = true,
        //        NextBuildTime = DateTime.MaxValue, // This will tell CCTray that the project isn't automatically triggered
        //        ShowStartStopButton = true
        //    };
        //    return projectStatus;
        //}
    }
}
