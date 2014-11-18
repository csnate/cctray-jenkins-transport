using JenkinsTransport.Interface;
using System;

namespace JenkinsTransport
{
    public class WebRequestFactory : IWebRequestFactory
    {
        public IWebRequest Create(string requestUriString)
        {
            return new WebRequestWrapper(requestUriString);
        }

        public IWebRequest Create(Uri requestUri)
        {
            return new WebRequestWrapper(requestUri);
        }
    }
}
