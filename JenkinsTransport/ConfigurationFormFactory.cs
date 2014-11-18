using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public class ConfigurationFormFactory : IFormFactory
    {
        public IForm Create()
        {
            return new ConfigurationForm();
        }
    }
}
