using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        public void SetConfiguration(BuildServer server)
        {
            Configuration = server;
        }

        /// <summary>
        /// Sets the session token
        /// </summary>
        /// <param name="session"></param>
        public void SetSessionToken(string session)
        {
            SessionToken = session;
        }

        #region ICruiseServerManager implmentations
        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            throw new NotImplementedException();
        }

        public CCTrayProject[] GetProjectList()
        {
            var api = new Api(Configuration.Url, AuthorizationInformation);
            var jobs = api.GetAllJobs();
            return jobs.Select(a => new CCTrayProject(Configuration, a.Name)
                                        {
                                            ShowProject = true
                                        }).ToArray();
        }

        public bool Login()
        {
            AuthorizationInformation = Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", Settings.Username, Settings.Password)));
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
        public Settings Settings { get; set; }

        /// <summary>
        /// The Basic AuthInfo header string constructed from the Settings U/P information.
        /// This is set on a call to Login
        /// </summary>
        public string AuthorizationInformation { get; private set; }
    }
}
