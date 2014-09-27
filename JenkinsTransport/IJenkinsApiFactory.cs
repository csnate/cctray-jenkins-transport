using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public interface IJenkinsApiFactory
    {        
        IJenkinsApi Create(string url, string authorizationInformation, IWebRequestFactory webRequestFactory);
    }
}
