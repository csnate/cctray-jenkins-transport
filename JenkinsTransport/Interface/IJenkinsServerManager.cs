﻿using System.Collections.Generic;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport.Interface
{
    /// <summary>
    /// Internal interface for abstraction of ServerManager
    /// </summary>
    public interface IJenkinsServerManager
    {
        Dictionary<string, ProjectStatus> ProjectsAndCurrentStatus { get; }
        CruiseServerSnapshot GetCruiseServerSnapshot();
        CruiseServerSnapshot GetCruiseServerSnapshotEx();
        void SetConfiguration(BuildServer server);
        CCTrayProject[] GetProjectList();
        void Initialize(BuildServer server, string session, string settings);
    }
}