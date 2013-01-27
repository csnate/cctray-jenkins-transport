using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace JenkinsTransport
{
    public class JenkinsTransportExtension : ITransportExtension
    {
        private const string Url = "http://build.office.comscore.com";

        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            return new CCTrayProject[0];
        }

        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            var manager = new JenkinsProjectManager();
            manager.SetProjectName(projectName);
            return manager;
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            var manager = new JenkinsServerManager();
            manager.SetConfiguration(Configuration);
            manager.SetSessionToken(String.Empty);
            return manager;
        }

        public bool Configure(IWin32Window owner)
        {
            // Create the Settings object
            var settings = new Settings()
                               {
                                   Project = "Build",
                                   Server = "CVIADPZB02"
                               };
            Configuration = new BuildServer(Url);
            Settings = settings.ToString();
            return true;
        }

        public string DisplayName { get { return "Jenkins Transport Extension"; } }
        public string Settings { get; set; }
        public BuildServer Configuration { get; set; }
    }
}
