using System;
using System.Collections.Generic;
using System.Linq;
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
}
