using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsTransport.UnitTests.ExtensionMethods
{
    internal static class JenkinsTransportExtensionTestExtenions
    {
        public static void SetIsServerManagerInitialized(this JenkinsTransportExtension instance, bool value)
        {
            var field = typeof(JenkinsTransportExtension).GetField("_isServerManagerInitialized", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Static);
            field.SetValue(instance, value);
        }  
    }
}
