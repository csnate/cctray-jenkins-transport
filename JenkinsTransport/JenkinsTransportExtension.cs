using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using JenkinsTransport.Interface;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport
{
    public class JenkinsTransportExtension : ITransportExtension
    {
        private IJenkinsServerManagerFactory _jenkinsServerManagerFactory;
        public IJenkinsServerManagerFactory JenkinsServerManagerFactory
        {
            get
            {
                if (_jenkinsServerManagerFactory == null)
                {
                    _jenkinsServerManagerFactory = new JenkinsServerManagerSingletonFactory(WebRequestFactory,
                    JenkinsApiFactory, DateTimeService);
                }
                return _jenkinsServerManagerFactory;
            }
            // Allow the JenkinsServerManagerFactory to be set for test purposes
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _jenkinsServerManagerFactory = value;
            }
        }

        private IWebRequestFactory _webRequestFactory;
        public IWebRequestFactory WebRequestFactory
        {
            get
            {
                // Use local default if none injected
                if (_webRequestFactory == null)
                {
                    _webRequestFactory = new WebRequestFactory();
                }
                return _webRequestFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _webRequestFactory = value;
            }
        }

        private IJenkinsApiFactory _jenkinsApiFactory;
        public IJenkinsApiFactory JenkinsApiFactory
        {
            get
            {
                // Use local default if none injected
                if (_jenkinsApiFactory == null)
                {
                    _jenkinsApiFactory = new JenkinsApiFactory();
                }
                return _jenkinsApiFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _jenkinsApiFactory = value;
            }
        }

        private IDateTimeService _dateTimeService;
        public IDateTimeService DateTimeService
        {
            get
            {
                // Lazy instantiation of default class
                if (_dateTimeService == null)
                {
                    _dateTimeService = new DateTimeService();
                }
                return _dateTimeService;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _dateTimeService = value;
            }
        }

        private IFormFactory _configurationFormFactory;
        public IFormFactory ConfigurationFormFactory
        {
            get
            {
                // Lazy instantiation of default class
                if (_configurationFormFactory == null)
                {
                    _configurationFormFactory = new ConfigurationFormFactory();
                }
                return _configurationFormFactory;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value"); 
                _configurationFormFactory = value;
            }
        }


        #region ITransportExtension implementations
        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            var manager = (IJenkinsServerManager)RetrieveServerManager();
            manager.SetConfiguration(server);

            // If we are getting the project list, then we should reset the ServerManager's project list
            // It will be reset when we return to the list
            manager.ProjectsAndCurrentStatus.Clear();

            return manager.GetProjectList();
        }

        // This is called once for every project that has been added to the user's CCtray instance
        //  Add each one to an internal property to be used when we get the server manager
        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            var manager = new JenkinsProjectManager(WebRequestFactory, JenkinsApiFactory);

            // Use factory to get reference to singleton ServerManager
            var serverManager = (IJenkinsServerManager)RetrieveServerManager();

            // Add this project to the server manager if it does not exist
            if (!serverManager.ProjectsAndCurrentStatus.ContainsKey(projectName))
            {
                serverManager.ProjectsAndCurrentStatus.Add(projectName, null);
            }

            // If this project does not have a status get it now as we need the WebURL
            if (serverManager.ProjectsAndCurrentStatus[projectName] == null)
            {
                serverManager.GetCruiseServerSnapshot();
            }

            if (serverManager.ProjectsAndCurrentStatus.ContainsKey(projectName) &&
                serverManager.ProjectsAndCurrentStatus[projectName] != null &&
                !String.IsNullOrEmpty(serverManager.ProjectsAndCurrentStatus[projectName].WebURL))
            {
                manager.WebURL = new Uri(serverManager.ProjectsAndCurrentStatus[projectName].WebURL);
            }
            else
            {
                // Really can't support nested jobs without knowning the exact WebURL for the project !!
            }

            manager.Initialize(Configuration, projectName, Settings);

            return manager;
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            var serverManager = JenkinsServerManagerFactory.GetInstance();
            if (!JenkinsServerManagerFactory.IsServerManagerInitialized)
            {
                serverManager.Initialize(Configuration, String.Empty, Settings);
                JenkinsServerManagerFactory.IsServerManagerInitialized = true;
            }
            return (ICruiseServerManager)serverManager;
        }

      
        public bool Configure(IWin32Window owner)
        {
            using (var form = ConfigurationFormFactory.Create())
            {
                if (form.ShowDialog(owner) == DialogResult.OK)
                {
                    var server = form.GetServer();
                    Configuration = new BuildServer(server);
                    var settings = new Settings()
                                        {
                                            Project = String.Empty,
                                            Server = server,
                                            Username = form.GetUsername(),
                                            Password = form.GetPassword()
                                        };
                    Settings = settings.ToString();
                    //We will need to initialize the server manager again if their information has changed
                    JenkinsServerManagerFactory.IsServerManagerInitialized = false; 
                    return true;
                }
                return false;
            }
        }

        public string DisplayName { get { return "Jenkins Transport Extension"; } }
        public string Settings { get; set; }
        public BuildServer Configuration { get; set; }

        

        #endregion
    }
}
