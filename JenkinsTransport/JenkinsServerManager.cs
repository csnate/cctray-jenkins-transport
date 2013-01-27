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

        public string DisplayName { get; private set; }
        public BuildServer Configuration { get; private set; }
        public string SessionToken { get; private set; }
    }
}
