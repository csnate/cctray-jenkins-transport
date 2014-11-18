using JenkinsTransport.Interface;
using System;
using System.Net;

namespace JenkinsTransport
{
    /// <summary>
    /// Wrapper around an underlying WebRequest instance exposed via IWebRequest
    /// </summary>
    public class WebRequestWrapper : IWebRequest
    {
        private readonly WebRequest _webRequest;

        public WebHeaderCollection Headers
        {
            get { return _webRequest.Headers; }
            set { _webRequest.Headers = value; }
        }

        public string Method 
        {
            get { return _webRequest.Method; }
            set { _webRequest.Method = value; } 
        }

        public WebResponse GetResponse()
        {
            return _webRequest.GetResponse();
        }

        public WebRequestWrapper(string requestUriString)
        {
            if (requestUriString == null) 
                throw new ArgumentNullException("requestUriString");

            _webRequest = WebRequest.Create(requestUriString);
        }

        public WebRequestWrapper(Uri requestUri)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            _webRequest = WebRequest.Create(requestUri);
        }
    }
}