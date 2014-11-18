using System.Reflection;

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
