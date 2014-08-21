using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using JenkinsTransport.Interface;

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

    /// <summary>
    /// Wrapper around an underlying WebRequest instance exposed via IWebRequest
    /// </summary>
    public class WebRequestWrapper : IWebRequest
    {
        private WebRequest _webRequest;

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
