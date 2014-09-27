using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public class JenkinsApiFactory : IJenkinsApiFactory
    {
        public IJenkinsApi Create(string url, string authorizationInformation, IWebRequestFactory webRequestFactory)
        {
            return new Api(url, authorizationInformation, webRequestFactory);
        }
    }
}