using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JenkinsTransport.Interface;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport
{
    public class JenkinsServerManager : ICruiseServerManager, IJenkinsServerManager
    {
        private readonly IWebRequestFactory _webRequestFactory;
        private readonly IDateTimeService _dateTimeService;
        private readonly IJenkinsApiFactory _apiFactory;

        private const int CACHE_INTERVAL_MILLISECONDS = 2000;

        private List<JenkinsJob> _allJobs;


        public JenkinsServerManager(IWebRequestFactory webRequestFactory, IJenkinsApiFactory apiFactory, IDateTimeService dateTimeService)
        {
            if (webRequestFactory == null) 
                throw new ArgumentNullException("webRequestFactory");

            if (apiFactory == null) 
                throw new ArgumentNullException("apiFactory");

            if (dateTimeService == null) 
                throw new ArgumentNullException("dateTimeService");

            _webRequestFactory = webRequestFactory;
            _apiFactory = apiFactory;
            _dateTimeService = dateTimeService;
        }

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
            Api = _apiFactory.Create(Configuration.Url, AuthorizationInformation, _webRequestFactory);

            ProjectsAndCurrentStatus = new Dictionary<string, ProjectStatus>();
        }

        #region ICruiseServerManager implmentations

        /// <summary>
        /// This only gets called while CCTray is polling for updates of known jobs
        /// </summary>
        /// <returns></returns>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            var projectStatues = AllJobs
                .Where(a => ProjectsAndCurrentStatus.ContainsKey(a.Name))
                .Select(a => Api.GetProjectStatus(a.Url, ProjectsAndCurrentStatus[a.Name]))
                .ToList();

            // Reset the ProjectsAndCurrentStatus dictionary
            ProjectsAndCurrentStatus = projectStatues.ToDictionary(a => a.Name);

            var snapshot = new CruiseServerSnapshot(projectStatues.ToArray(), new QueueSetSnapshot());
            return snapshot;
        }

        private void UpdateAllJobsIfCacheExpired()
        {
            if (HasCacheExpired())
            {
                AllJobsLastUpdate = _dateTimeService.Now;
                AllJobs = Api.GetAllJobs();    
            }            
        }

        private bool HasCacheExpired()
        {
            return (TimeSinceLastUpdate() > TimeSpan.FromMilliseconds(CACHE_INTERVAL_MILLISECONDS));
        }

        private TimeSpan TimeSinceLastUpdate()
        {
            return _dateTimeService.Now - AllJobsLastUpdate;
        }

        public CCTrayProject[] GetProjectList()
        {
            UpdateAllJobsIfCacheExpired();

            return AllJobs.Select(a => new CCTrayProject(Configuration, a.Name)
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
        /// The Api
        /// </summary>
        protected IJenkinsApi Api { get; private set; }

        /// <summary>
        /// The Settings. Passed from the JenkinsTransportExtension.
        /// </summary>
        public Settings Settings { get; private set; }

        /// <summary>
        /// The Basic AuthInfo header string constructed from the Settings U/P information.
        /// This is set on a call to Login
        /// </summary>
        public string AuthorizationInformation { get; private set; }

        /// <summary>
        /// The list of projects configured/set for this server
        /// </summary>
        public Dictionary<string, ProjectStatus> ProjectsAndCurrentStatus { get; private set; }

        public DateTime AllJobsLastUpdate { get; set; }

        /// <summary>
        /// The current list of jobs for this server
        /// </summary>
        public List<JenkinsJob> AllJobs
        {
            get { return _allJobs ?? (_allJobs = Api.GetAllJobs()); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _allJobs = value;
            }
        }
    }
}
