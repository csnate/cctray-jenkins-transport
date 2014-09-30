using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsTransport.UnitTests.ExtensionMethods
{
    public static class JenkinsServerManagerTestExtensions
    {
        public static void SetAllJobs(this JenkinsServerManager server, List<JenkinsJob> _allJobs)
        {
            var field = typeof(JenkinsServerManager).GetField("_allJobs", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            field.SetValue(server, _allJobs);
        }
    }
}
