using System.Collections.Generic;
using System.Reflection;

namespace JenkinsTransport.UnitTests.ExtensionMethods
{
    internal static class JenkinsServerManagerTestExtensions
    {
        public static void SetAllJobs(this JenkinsServerManager server, List<JenkinsJob> _allJobs)
        {
            var field = typeof(JenkinsServerManager).GetField("_allJobs", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            field.SetValue(server, _allJobs);
        }        
    }
}
