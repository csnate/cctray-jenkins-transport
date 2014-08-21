using System;
using System.Collections.Generic;
using System.Text;
using JenkinsTransport.Interface;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class JenkinsProjectManager : ICruiseProjectManager
    {
        private readonly IWebRequestFactory _webRequestFactory;

        public JenkinsProjectManager(IWebRequestFactory webRequestFactory)
        {
            _webRequestFactory = webRequestFactory;
        }

        /// <summary>
        /// The Api
        /// </summary>
        protected Api Api { get; private set; }

        /// <summary>
        /// Sets the Configuration for this server manager
        /// </summary>
        /// <param name="server">the BuildServer configuration</param>
        /// <param name="projectName">the project name</param>
        /// <param name="settings">the Settings in string form</param>
        public void Initialize(BuildServer server, string projectName, string settings)
        {
            Initialize(server, projectName, Settings.GetSettings(settings));
        }

        /// <summary>
        /// Sets the Configuration for this server manager
        /// </summary>
        /// <param name="server">the BuildServer configuration</param>
        /// <param name="projectName">the project name</param>
        /// <param name="settings">the Settings</param>
        public void Initialize(BuildServer server, string projectName, Settings settings)
        {
            Configuration = server;
            ProjectName = projectName;
            Settings = settings;
            AuthorizationInformation = Settings.AuthorizationInformation;
            //Api = new Api(Settings.Server, AuthorizationInformation);
            Api = new ApiNonStatic(Settings.Server, AuthorizationInformation, _webRequestFactory);
        }

        #region ICruiseProjectManager implmentations
        public void ForceBuild(string sessionToken, Dictionary<string, string> parameters, string userName)
        {
            Api.ForceBuild(ProjectName, parameters);
        }

        public void AbortBuild(string sessionToken, string userName)
        {
            Api.AbortBuild(ProjectName);
        }

        public void StopProject(string sessionToken)
        {
            Api.StopProject(ProjectName);
        }

        public void StartProject(string sessionToken)
        {
            Api.StartProject(ProjectName);
        }

        public ProjectStatusSnapshot RetrieveSnapshot()
        {
            return Api.GetProjectStatusSnapshot(ProjectName);
        }

        public List<ParameterBase> ListBuildParameters()
        {
            // CCTray calls this method with every call to ForceBuild
            return Api.GetBuildParameters(ProjectName);
        }

        #region Not Implemented
        public void CancelPendingRequest(string sessionToken)
        {
            throw new NotImplementedException();
        }

        public PackageDetails[] RetrievePackageList()
        {
            throw new NotImplementedException();
        }

        public IFileTransfer RetrieveFileTransfer(string fileName)
        {
            throw new NotImplementedException();
        }

        public void FixBuild(string sessionToken, string fixingUserName)
        {
            throw new NotImplementedException();
        }
        #endregion

        public string ProjectName { get; private set; }
        #endregion

        /// <summary>
        /// The Settings. Passed from the JenkinsTransportExtension.
        /// </summary>
        public Settings Settings { get; private set; }

        /// <summary>
        /// The BuildServer passed from the JenkinsTransportExtension.
        /// </summary>
        public BuildServer Configuration { get; private set; }

        /// <summary>
        /// The Basic AuthInfo header string constructed from the Settings U/P information.
        /// This is set on a call to Login
        /// </summary>
        public string AuthorizationInformation { get; private set; }
    }
}
