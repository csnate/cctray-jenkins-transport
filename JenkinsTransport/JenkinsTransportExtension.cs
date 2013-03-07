using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport
{
    public class JenkinsTransportExtension : ITransportExtension
    {
        private static bool _isServerManagerInitialized = false;
        private static JenkinsServerManager _jenkinsServerManager;

        protected static JenkinsServerManager JenkinsServerManager
        {
            get { return _jenkinsServerManager ?? (_jenkinsServerManager = new JenkinsServerManager()); }
        }

        protected static Configuration GetApplicationConfiguration(string configFile = null)
        {
            if (String.IsNullOrEmpty(configFile))
            {
                configFile = Assembly.GetExecutingAssembly().CodeBase + ".config";
                if (configFile.Contains("file:///"))
                {
                    configFile = configFile.Replace("file:///", String.Empty);
                }
            }

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("No configuration file found for the JenkinsTransportExtension. A JenkinsTransport.dll.config file must be present in the same directory as the assembly.");
            }

            return ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                                                                       {
                                                                           ExeConfigFilename = configFile
                                                                       }, ConfigurationUserLevel.None);
        }

        protected static BuildServer GetBuildServerFromConfiguration(Configuration configuration = null)
        {
            if (configuration == null)
            {
                configuration = GetApplicationConfiguration();
            }
            var buildServer = new BuildServer(configuration.AppSettings.Settings["ServerUrlRoot"].Value);
            return buildServer;
        }

        protected static Settings GetSettingsFromConfiguration(Configuration configuration = null)
        {
            if (configuration == null)
            {
                configuration = GetApplicationConfiguration();
            }
            return new Settings()
            {
                Project = String.Empty,
                Server = configuration.AppSettings.Settings["ServerUrlRoot"].Value,
                Username = configuration.AppSettings.Settings["Username"].Value,
                Password = configuration.AppSettings.Settings["Password"].Value
            };
        }

        /// <summary>
        /// Tells if we should use a config file to get the configuration information. Change for Unit Tests.
        /// </summary>
        public bool UseConfigurationFile = true;

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
            var manager = new JenkinsProjectManager();
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
            var form = new ConfigurationForm();
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
                return true;
            }

            return false;
        }

        public string DisplayName { get { return "Jenkins Transport Extension"; } }
        public string Settings { get; set; }
        public BuildServer Configuration { get; set; }
        #endregion
    }
}
