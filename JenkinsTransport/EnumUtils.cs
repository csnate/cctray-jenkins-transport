using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;

namespace JenkinsTransport
{
    public class EnumUtils
    {
        // Map of the possible Jenkins status with CCTray Integration Status
        private static readonly Dictionary<string, IntegrationStatus> BuildStatusMap = new Dictionary<string, IntegrationStatus>()
                                                           {
                                                               { "blue", IntegrationStatus.Success },
                                                               { "yellow", IntegrationStatus.Exception },
                                                               { "red", IntegrationStatus.Failure},
                                                               { "grey", IntegrationStatus.Unknown },
                                                               { "disabled", IntegrationStatus.Unknown},
                                                               { "blue_anime", IntegrationStatus.Success },
                                                               { "yellow_anime", IntegrationStatus.Exception },
                                                               { "red_anime", IntegrationStatus.Failure},
                                                               { "grey_anime", IntegrationStatus.Unknown },
                                                               { "disabled_anime", IntegrationStatus.Unknown}
                                                           };

        // Map of the possible Jenkins status with CCTray ProjectActivity status
        private static readonly Dictionary<string, ProjectActivity> ActivityMap = new Dictionary<string, ProjectActivity>()
                                                                      {
                                                                           { "blue", ProjectActivity.Sleeping },
                                                                           { "yellow", ProjectActivity.Sleeping },
                                                                           { "red", ProjectActivity.Sleeping},
                                                                           { "grey", ProjectActivity.Sleeping },
                                                                           { "disabled", ProjectActivity.Sleeping },
                                                                           { "blue_anime", ProjectActivity.Building },
                                                                           { "yellow_anime", ProjectActivity.Building },
                                                                           { "red_anime", ProjectActivity.Building},
                                                                           { "grey_anime", ProjectActivity.Building },
                                                                           { "disabled_anime", ProjectActivity.Building}
                                                                      };

        /// <summary>
        /// Gets the correct IntegrationStatus for the specified Jenkins build color
        /// </summary>
        /// <param name="color">the color of the build</param>
        public static IntegrationStatus GetIntegrationStatus(string color)
        {
            return BuildStatusMap.ContainsKey(color) ? BuildStatusMap[color] : IntegrationStatus.Unknown;
        }

        /// <summary>
        /// Gets the correct ProjectActivity for the specified Jenkins build color
        /// </summary>
        /// <param name="color">the color of the build</param>
        public static ProjectActivity GetProjectActivity(string color)
        {
            return ActivityMap.ContainsKey(color) ? ActivityMap[color] : ProjectActivity.Sleeping;
        }

        /// <summary>
        /// Gets the correct ProjectIntegratorState for the specified Jenkins build color
        /// </summary>
        /// <param name="color">the color of the build</param>
        public static ProjectIntegratorState GetProjectIntegratorState(string color)
        {
            return color == "disabled" ? ProjectIntegratorState.Stopped : ProjectIntegratorState.Running;
        }
        
        /// <summary>
        /// Gets the correct ProjectIntegratorState for the specified Jenkins buildable value. This is the preferred method
        /// </summary>
        /// <param name="buildable">the current buildable state</param>
        public static ProjectIntegratorState GetProjectIntegratorState(bool buildable)
        {
            return buildable ? ProjectIntegratorState.Running : ProjectIntegratorState.Stopped;
        }
    }
}
