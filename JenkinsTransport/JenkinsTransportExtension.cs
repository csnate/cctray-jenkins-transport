using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static bool _isServerManagerInitialized = false;
        private static JenkinsServerManager _jenkinsServerManager;

        private IWebRequestFactory _webRequestFactory;
        protected IWebRequestFactory WebRequestFactory
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
                    throw new NullReferenceException();
                _webRequestFactory = value;
            }
        }

        protected static JenkinsServerManager JenkinsServerManager
        {
            get { return _jenkinsServerManager ?? (_jenkinsServerManager = new JenkinsServerManager(new WebRequestFactory())); }
        }

        #region ITransportExtension implementations
        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            var manager = (JenkinsServerManager)RetrieveServerManager();
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
            var manager = new JenkinsProjectManager(_webRequestFactory);
            manager.Initialize(Configuration, projectName, Settings);

            // Check to make sure the static instance of JenkinsServerManager is initialized
            var serverManager = (JenkinsServerManager)RetrieveServerManager();

            // Add this project to the server manager
            if (!serverManager.ProjectsAndCurrentStatus.ContainsKey(projectName))
            {
                serverManager.ProjectsAndCurrentStatus.Add(projectName, null);
            }

            return manager;
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            if (!_isServerManagerInitialized)
            {
                JenkinsServerManager.Initialize(Configuration, String.Empty, Settings);
                _isServerManagerInitialized = true;
            }
            return JenkinsServerManager;
        }

        public bool Configure(IWin32Window owner)
        {
            using (var form = new ConfigurationForm())
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
                    _isServerManagerInitialized = false;  // We will need to initialize the server manager again if their information has changed
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
