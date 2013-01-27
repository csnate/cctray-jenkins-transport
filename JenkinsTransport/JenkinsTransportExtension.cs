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
        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            var list = new CCTrayProject[0];
            var a = new CCTrayProject("http://www.asd.com", DisplayName + " SERVER");
            list[0] = a;
            return list;
        }

        public ICruiseProjectManager RetrieveProjectManager(string projectName)
        {
            return new JenkinsProjectManager();
        }

        public ICruiseServerManager RetrieveServerManager()
        {
            return new JenkinsServerManager();
        }

        public bool Configure(IWin32Window owner)
        {
            return true;
        }

        public string DisplayName { get { return "Jenkins Transport Extension"; } }
        public string Settings { get; set; }
        public BuildServer Configuration { get; set; }
    }
}
