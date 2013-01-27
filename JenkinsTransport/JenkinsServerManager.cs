using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport
{
    public class JenkinsServerManager : ICruiseServerManager
    {
        /// <summary>
        /// Sets the Configuration for this server manager
        /// </summary>
        /// <param name="server"></param>
        internal void SetConfiguration(BuildServer server)
        {
            Configuration = server;
        }

        /// <summary>
        /// Sets the session token
        /// </summary>
        /// <param name="session"></param>
        internal void SetSessionToken(string session)
        {
            SessionToken = session;
        }

        #region ICruiseServerManager implmentations
        public void CancelPendingRequest(string projectName)
        {
            throw new NotImplementedException();
        }

        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            throw new NotImplementedException();
        }

        public CCTrayProject[] GetProjectList()
        {
            throw new NotImplementedException();
        }

        public bool Login()
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }

        public string DisplayName
        {
            get { return Configuration != null ? Configuration.DisplayName : String.Empty; }
        }
        
        public BuildServer Configuration { get; private set; }
        public string SessionToken { get; private set; }
        #endregion
    }
}
