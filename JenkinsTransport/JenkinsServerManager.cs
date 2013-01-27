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

        private string GetXml(string url)
        {
            var request = WebRequest.Create(url);
            var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(String.Format("{0}:{1}", Settings.Username, Settings.Password)));
            request.Headers["Authorization"] = "Basic " + authInfo;
            request.Method = "GET";

            var result = String.Empty;
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var rd = new StreamReader(responseStream))
                        {
                            result = rd.ReadToEnd();
                        }
                    }
                }
            }
            return result;
        }

        public Settings Settings { get; set; }

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
            var xmlString = GetXml(Configuration.Url + "/cc.xml");
            var projects = (DashboardProjects) XmlConversionUtil.ConvertXmlToObject(typeof(DashboardProjects), xmlString);
            return projects.Projects.Select(a => new CCTrayProject(a.webUrl, a.name)).ToArray();
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
