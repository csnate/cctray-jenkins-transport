using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using JenkinsTransport.BuildParameters;
using JenkinsTransport.Interface;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class Api : IJenkinsApi
    {
        protected readonly IWebRequestFactory WebRequestFactory;

        #region Constants
        protected const string XmlApi = "/api/xml";
        protected const string AllJobs = XmlApi + "?depth=1&xpath=/hudson/job&wrapper=jobs";
        protected const string ExcludeBuild = XmlApi + "?exclude=freeStyleProject/build&exclude=freeStyleProject/healthReport&exclude=freeStyleProject/action";
        protected const string ForceBuildParams = "/build?delay=0sec";
        protected const string ForceBuildWithParametersParams = "/buildWithParameters";
        protected const string StopProjectParams = "/disable";
        protected const string StartProjectParams = "/enable";
        #endregion

        protected string BaseUrl { get; set; }
        protected string ProjectBaseUrl { get; private set; }
        protected string AuthInfo { get; set; }

        protected XDocument GetXDocument(string url, string authInfo)
        {
            var request = WebRequestFactory.Create(url);

            if (!String.IsNullOrEmpty(authInfo))
            {
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
            request.Method = "GET";

            return GetXDocument(request);
        }

        protected XDocument GetXDocument(IWebRequest request)
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

        protected void MakeRequest(string url, string method = "POST", byte[] postData = null)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            if (!String.IsNullOrEmpty(AuthInfo))
            {
                request.Headers["Authorization"] = "Basic " + AuthInfo;
            }

            // Fake the referer
            request.Referer = BaseUrl;
            request.Method = method;
            if (method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";

                // If we need to supply post data, then we have to write it to the response stream
                if (postData != null)
                {
                    request.ContentLength = postData.Length;
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(postData, 0, postData.Length);
                    }
                }
            }

            // I don't care what is returned, but the response should be disposed
            using (var response = request.GetResponse()) { }  
            
        }

        public Api(string baseUrl, string authInfo, IWebRequestFactory webRequestFactory)
        {
            if (webRequestFactory == null) 
                throw new ArgumentNullException("webRequestFactory");

            WebRequestFactory = webRequestFactory;
            BaseUrl = baseUrl;
            ProjectBaseUrl = baseUrl + "/job/";
            AuthInfo = authInfo;
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        public List<JenkinsJob> GetAllJobs()
        {
            var xDoc = GetXDocument(BaseUrl + AllJobs, AuthInfo);
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
            var xDoc = GetXDocument(projectUrl + ExcludeBuild, AuthInfo);
            return GetProjectStatus(xDoc, currentStatus);
        }

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        /// <param name="currentStatus">the current stored status</param>
        public ProjectStatus GetProjectStatus(XDocument xDoc, ProjectStatus currentStatus)
        {
            // The first element in the document is named based on the type of project (freeStyleProject, mavenModuleSet, etc). Can't access based on name.
            var firstElement = xDoc.Descendants().First<XElement>(); 
            var lastBuildElement = firstElement.Element("lastBuild");  // Will contain the latest (in progress) build number

            // GUARD
            if (!HasLastBuildNumberChanged(currentStatus, lastBuildElement))
            {
                return currentStatus;
            }

            var color = (string)firstElement.Element("color");            
            var lastSuccessfulBuildElement = firstElement.Element("lastSuccessfulBuild");
            var lastCompletedBuildElement = firstElement.Element("lastCompletedBuild");

            // Otherwise, we'll need to get the new status of the last completed build
            JenkinsBuildInformation lastCompletedBuildInfo = new JenkinsBuildInformation();
            if (lastCompletedBuildElement != null)
            {
                lastCompletedBuildInfo = GetBuildInformation((string) lastCompletedBuildElement.Element("url"));
            }
            
            string lastSuccessfulBuildNumber = String.Empty;
            if (lastSuccessfulBuildElement != null)
            {
                lastSuccessfulBuildNumber = lastSuccessfulBuildElement.Element("number").Value;
            }
            
            var name = (string) firstElement.Element("name");
            var projectStatus = new ProjectStatus(name, EnumUtils.GetIntegrationStatus(color),
                                                  lastCompletedBuildInfo.Timestamp)
                                    {
                                        Activity = EnumUtils.GetProjectActivity(color),
                                        Status = EnumUtils.GetProjectIntegratorState((bool)firstElement.Element("buildable")),
                                        WebURL = (string) firstElement.Element("url"),
                                        LastBuildLabel = lastCompletedBuildInfo.Number,
                                        LastSuccessfulBuildLabel = lastSuccessfulBuildNumber,
                                        Queue = name,
                                        QueuePriority = 0,
                                        Description = (string)firstElement.Element("description"),
                                        ShowForceBuildButton = true,
                                        NextBuildTime = DateTime.MaxValue, // This will tell CCTray that the project isn't automatically triggered
                                        ShowStartStopButton = true
                                    };

            return projectStatus;
        }


        private bool HasLastBuildNumberChanged(ProjectStatus currentStatus, XElement lastBuildElement)
        {
            if (currentStatus == null)
                return true;

            if (lastBuildElement == null)
                return true;

            return currentStatus.LastBuildLabel != (string)lastBuildElement.Element("number");
        }

        /// <summary>
        /// Get the build information for a build information url
        /// </summary>
        /// <param name="buildInformationUrl">the build information url, without /api/xml</param>
        public JenkinsBuildInformation GetBuildInformation(string buildInformationUrl)
        {
            var xDoc = GetXDocument(buildInformationUrl + XmlApi, AuthInfo);

            return new JenkinsBuildInformation(xDoc);
        }

        public XDocument GetBuildInformationDoc(string buildInformationUrl)
        {
            return GetXDocument(buildInformationUrl + XmlApi, AuthInfo);
        }

        
        /// <summary>
        /// Returns the build parameters for a project
        /// </summary>
        /// <param name="projectName">the project name</param>
        public List<ParameterBase> GetBuildParameters(string projectName)
        {
            var buildProjectUrl = ProjectBaseUrl + HttpUtility.HtmlEncode(projectName);
            var xDoc = GetXDocument(buildProjectUrl + XmlApi, AuthInfo);

            return GetBuildParameters(xDoc);          
        }

        public List<ParameterBase> GetBuildParameters(Uri projectUri)
        {
            var buildProjectUrl = projectUri;
            var xDoc = GetXDocument(buildProjectUrl + XmlApi, AuthInfo);

            return GetBuildParameters(xDoc);
        }

        private List<ParameterBase> GetBuildParameters(XDocument xDoc)
        {
            // Construct the build parameters
            var buildParameters = new List<ParameterBase>();
            var parametersNodes = xDoc.Descendants("action").Elements("parameterDefinition");
            var supportedTypes = Enum.GetNames(typeof(BuildParameterType));
            foreach (var parameterNode in parametersNodes)
            {
                var type = (string)parameterNode.Element("type");
                if (!supportedTypes.Contains(type)) continue;

                switch ((BuildParameterType)Enum.Parse(typeof(BuildParameterType), type))
                {
                    case BuildParameterType.BooleanParameterDefinition:
                        var booleanBuildParamter = new BuildParameters.BooleanParameter(parameterNode);
                        buildParameters.Add(booleanBuildParamter.ToParameterBase());
                        break;

                    case BuildParameterType.ChoiceParameterDefinition:
                        var choiceBuildParameter = new ChoiceParameter(parameterNode);
                        buildParameters.Add(choiceBuildParameter.ToParameterBase());
                        break;

                    case BuildParameterType.StringParameterDefinition:
                        var stringBuildParameter = new StringParameter(parameterNode);
                        buildParameters.Add(stringBuildParameter.ToParameterBase());
                        break;
                }
            }

            return buildParameters;
        }

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="projectName">the project name to check</param>
        public ProjectStatusSnapshot GetProjectStatusSnapshot(string projectName)
        {
            var url = ProjectBaseUrl + HttpUtility.HtmlEncode(projectName) + ExcludeBuild;
            var xDoc = GetXDocument(url, AuthInfo);
            return GetProjectStatusSnapshot(xDoc);
        }

        public ProjectStatusSnapshot GetProjectStatusSnapshot(Uri projectUrl)
        {
            var url = projectUrl + ExcludeBuild;
            var xDoc = GetXDocument(url, AuthInfo);
            return GetProjectStatusSnapshot(xDoc);
        }

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="xDoc">the XDcoument to parse</param>
        public ProjectStatusSnapshot GetProjectStatusSnapshot(XDocument xDoc)
        {
            var firstElement = xDoc.Descendants().First<XElement>();  // The first element in the document is named based on the type of project (freeStyleProject, mavenModuleSet, etc)
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

        // --- Project specific apis

        public void ForceBuild(Uri projectUrl)
        {
            MakeRequest(projectUrl + ForceBuildParams);
        }
        /// <summary>
        /// Forces a build of a project
        /// </summary>
        /// <param name="projectName">the project name to build</param>
        public void ForceBuild(string projectName)
        {
            MakeRequest(ProjectBaseUrl + projectName + ForceBuildParams);
        }

        /// <summary>
        /// Forces a build of a project with parameters
        /// </summary>
        /// <param name="projectName">the project name</param>
        /// <param name="parameters">the parameters to the build</param>
        public void ForceBuild(string projectName, Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                ForceBuild(projectName);
                return;
            }

            // Construct the query string
            var queryString = new StringBuilder();
            foreach (var parameter in parameters)
            {
                queryString.Append(parameter.Key + "=" + parameter.Value + "&");
            }
            queryString.Remove(queryString.Length - 1, 1);

            // I've see some Jenkins instances require this to be a POST request if Authentication is supplied.
            // Check if Auth info is empty and make the appropriate change to the method and post data
            if (String.IsNullOrEmpty(AuthInfo))
            {
                MakeRequest(ProjectBaseUrl + projectName + ForceBuildWithParametersParams + "?" + queryString.ToString(), "GET");
            }
            else
            {
                var postByteData = Encoding.UTF8.GetBytes(queryString.ToString());

                // With POST requests, the server will sometimes return a 403 Forbidden response
                // However, the build still goes through correctly, so I'm catching and ignoring any WebExecptions with response code of 403
                try
                {
                    MakeRequest(ProjectBaseUrl + projectName + ForceBuildWithParametersParams, "POST", postByteData);
                }
                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.ProtocolError || !e.Message.Contains("403"))
                    {
                        throw;
                    }
                }
                
            }
        }

        /// <summary>
        /// Abort the latest build
        /// </summary>
        /// <param name="projectName">the project name to abort</param>
        public void AbortBuild(string projectName)
        {
            // Need to get the last build number/url
            var projectUrl = ProjectBaseUrl + projectName;
            var xDoc = GetXDocument(projectUrl + ExcludeBuild, AuthInfo);

            AbortBuild(xDoc);          
        }

        public void AbortBuild(Uri projectUrl)
        {
            // Need to get the last build number/url
            var xDoc = GetXDocument(projectUrl + ExcludeBuild, AuthInfo);

            AbortBuild(xDoc);
        }

        private void AbortBuild(XDocument xDoc)
        {
            var lastBuildUrl = (string)xDoc.Descendants().First<XElement>().Element("lastBuild").Element("url");

            MakeRequest(lastBuildUrl + "stop");
        }

        /// <summary>
        /// Stops (disables) a project
        /// </summary>
        /// <param name="projectName">the project name to disable</param>
        public void StopProject(string projectName)
        {
            MakeRequest(ProjectBaseUrl + projectName + StopProjectParams);
        }

        /// <summary>
        /// Starts (enables) a project
        /// </summary>
        /// <param name="projectName"></param>
        public void StartProject(string projectName)
        {
            MakeRequest(ProjectBaseUrl + projectName + StartProjectParams);
        }

        public void StartProject(Uri projectUrl)
        {
            MakeRequest(projectUrl + StartProjectParams);
        }
    }
}
