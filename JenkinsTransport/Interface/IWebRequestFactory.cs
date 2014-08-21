using System;

namespace JenkinsTransport.Interface
{
    public interface IWebRequestFactory
    {
        IWebRequest Create(string requestUriString);
        IWebRequest Create(Uri requestUri);
    }
}