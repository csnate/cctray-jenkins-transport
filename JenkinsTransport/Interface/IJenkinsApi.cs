using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.Interface
{
    public interface IJenkinsApi
    {
        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        List<JenkinsJob> GetAllJobs();

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        List<JenkinsJob> GetAllJobs(XDocument xDoc);

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="projectUrl">the project url to retrieve the info</param>
        /// <param name="currentStatus">the current stored status</param>
        ProjectStatus GetProjectStatus(string projectUrl, ProjectStatus currentStatus);

        /// <summary>
        /// Get the project status for a project
        /// </summary>
        /// <param name="xDoc">the XDocument to parse</param>
        /// <param name="currentStatus">the current stored status</param>
        ProjectStatus GetProjectStatus(XDocument xDoc, ProjectStatus currentStatus);

        /// <summary>
        /// Get the build information for a build information url
        /// </summary>
        /// <param name="buildInformationUrl">the build information url, without /api/xml</param>
        JenkinsBuildInformation GetBuildInformation(string buildInformationUrl);

        XDocument GetBuildInformationDoc(string buildInformationUrl);

        /// <summary>
        /// Returns the build parameters for a project
        /// </summary>
        /// <param name="projectName">the project name</param>
        List<ParameterBase> GetBuildParameters(string projectName);

        List<ParameterBase> GetBuildParameters(Uri projectUri);

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="projectName">the project name to check</param>
        ProjectStatusSnapshot GetProjectStatusSnapshot(string projectName);

        ProjectStatusSnapshot GetProjectStatusSnapshot(Uri projectUrl);

        /// <summary>
        /// Get the project snapshot for a project
        /// </summary>
        /// <param name="xDoc">the XDcoument to parse</param>
        ProjectStatusSnapshot GetProjectStatusSnapshot(XDocument xDoc);

        /// <summary>
        /// Forces a build of a project
        /// </summary>
        /// <param name="projectName">the project name to build</param>
        void ForceBuild(string projectName);

        /// <summary>
        /// Forces a build of a project
        /// </summary>
        /// <param name="projectUrl">the project url to build</param>
        void ForceBuild(Uri projectUrl);

        /// <summary>
        /// Forces a build of a project with parameters
        /// </summary>
        /// <param name="projectName">the project name</param>
        /// <param name="parameters">the parameters to the build</param>
        void ForceBuild(string projectName, Dictionary<string, string> parameters);

        /// <summary>
        /// Forces a build of a project with parameters
        /// </summary>
        /// <param name="projectUrl">the project url to build</param>
        /// <param name="parameters">the parameters to the build</param>
        void ForceBuild(Uri projectUrl, Dictionary<string, string> parameters);

        /// <summary>
        /// Abort the latest build
        /// </summary>
        /// <param name="projectName">the project name to abort</param>
        void AbortBuild(string projectName);

        /// <summary>
        /// Abort the latest build
        /// </summary>
        /// <param name="projectUrl">the project url to abort</param>
        void AbortBuild(Uri projectUrl);

        /// <summary>
        /// Stops (disables) a project
        /// </summary>
        /// <param name="projectName">the project name to disable</param>
        void StopProject(string projectName);

        /// <summary>
        /// Starts (enables) a project
        /// </summary>
        /// <param name="projectName"></param>
        void StartProject(string projectName);

        void StartProject(Uri projectUrl);
        
    }
}