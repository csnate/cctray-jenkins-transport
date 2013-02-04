using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class JenkinsServerManager : ICruiseServerManager
    {
        /// <summary>
        /// The Api
        /// </summary>
        protected Api Api { get; private set; }

        /// <summary>
        /// Sets the configuration
        /// </summary>
        /// <param name="server">the BuildServer</param>
        public void SetConfiguration(BuildServer server)
        {
            Configuration = server;
        }

        /// <summary>
        /// Initializes this instance with the appropriate information
        /// </summary>
        /// <param name="server">the BuildServer</param>
        /// <param name="session">the SessionToken</param>
        /// <param name="settings">the Settings in string form</param>
        public void Initialize(BuildServer server, string session, string settings)
        {
            Initialize(server, session, Settings.GetSettings(settings));
        }

        /// <summary>
        /// Initializes this instance with the appropriate information
        /// </summary>
        /// <param name="server">the BuildServer</param>
        /// <param name="session">the SessionToken</param>
        /// <param name="settings">the Settings</param>
        public void Initialize(BuildServer server, string session, Settings settings)
        {
            Configuration = server;
            SessionToken = session;
            Settings = settings;
            Login();
            Api = new Api(Configuration.Url, AuthorizationInformation);
        }

        #region ICruiseServerManager implmentations
        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            var jobs = Api.GetAllJobs();
            var projectStatues = jobs.Select(jenkinsJob => Api.GetProjectStatus(jenkinsJob.Url)).ToArray();

            var snapshot = new CruiseServerSnapshot(projectStatues, new QueueSetSnapshot());
            return snapshot;
        }

        public CCTrayProject[] GetProjectList()
        {
            var jobs = Api.GetAllJobs();
            return jobs.Select(a => new CCTrayProject(Configuration, a.Name)
                                        {
                                            ShowProject = a.Color != "disabled"
                                        }).ToArray();
        }

        public bool Login()
        {
            AuthorizationInformation = Settings.AuthorizationInformation;
            return true;
        }

        public void Logout()
        {
            AuthorizationInformation = String.Empty;
        }

        public string DisplayName
        {
            get { return Configuration != null ? Configuration.DisplayName : String.Empty; }
        }
        
        public BuildServer Configuration { get; private set; }
        public string SessionToken { get; private set; }

        #region Not Implemented
        public void CancelPendingRequest(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        /// <summary>
        /// The Settings. Passed from the JenkinsTransportExtension.
        /// </summary>
        public Settings Settings { get; private set; }

        /// <summary>
        /// The Basic AuthInfo header string constructed from the Settings U/P information.
        /// This is set on a call to Login
        /// </summary>
        public string AuthorizationInformation { get; private set; }
    }
}
