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
        public static Configuration GetApplicationConfiguration(string configFile = null)
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

        public static BuildServer GetBuildServerFromConfiguration(Configuration configuration = null)
        {
            if (configuration == null)
            {
                configuration = GetApplicationConfiguration();
            }
            var buildServer = new BuildServer(configuration.AppSettings.Settings["ServerUrlRoot"].Value);
            return buildServer;
        }

        public static Settings GetSettingsFromConfiguration(Configuration configuration = null)
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
            return manager.GetProjectList();
        }

        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            var manager = new JenkinsProjectManager();
            manager.SetProjectName(projectName);
            return manager;
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            var manager = new JenkinsServerManager()
                              {
                                  Settings = JenkinsTransport.Settings.GetSettings(Settings)
                              };
            manager.SetConfiguration(Configuration);
            manager.SetSessionToken(String.Empty);
            manager.Login();  // This call is needed here intially. Should be overriden when we configure it again
            return manager;
        }

        public bool Configure(IWin32Window owner)
        {
            // Create the Settings object
            if (UseConfigurationFile)
            {
                var config = GetApplicationConfiguration();
                Configuration = GetBuildServerFromConfiguration(config);
                Settings = GetSettingsFromConfiguration(config).ToString();
            }
            else
            {
                Configuration = new BuildServer("http://add.a.configuration.file.com");
                Settings = String.Empty;
            }
            
            return true;
        }

        public string DisplayName { get { return "Jenkins Transport Extension"; } }
        public string Settings { get; set; }
        public BuildServer Configuration { get; set; }
        #endregion
    }
}
