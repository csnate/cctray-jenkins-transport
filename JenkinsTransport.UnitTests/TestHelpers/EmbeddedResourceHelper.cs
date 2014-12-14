using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    public static class EmbeddedResourceHelper
    {
        public static void ExtractManifestResourceToDisk(string relativeManifestUri, string targetPath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyNamespace = assembly.GetName().Name;

            //if (File.Exists(targetPath))
            //    return;

            var targetFolder = Path.GetDirectoryName(targetPath);
            Directory.CreateDirectory(targetFolder);
            // Return all files contained in this assembly
            string[] names = assembly.GetManifestResourceNames();

            var uri = String.Format("{0}.{1}", assembly.GetName().Name, relativeManifestUri);

            using (Stream input = assembly.GetManifestResourceStream(uri))            
                using (Stream output = File.Create(targetPath))
                {
                    input.CopyTo(output);
                }
            
        }
    }
}