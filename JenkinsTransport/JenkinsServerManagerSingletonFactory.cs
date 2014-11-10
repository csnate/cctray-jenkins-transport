using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public class JenkinsServerManagerSingletonFactory : IJenkinsServerManagerFactory
    {
        private static JenkinsServerManager _instance;
        private static bool _isServerManagerInitialized;

        private readonly IWebRequestFactory _webRequestFactory;
        private readonly IJenkinsApiFactory _jenkinsApiFactory;
        private readonly IDateTimeService _dateTimeService;
        
        public bool IsServerManagerInitialized
        {
            get { return _isServerManagerInitialized; }
            set { _isServerManagerInitialized = value; }
        }

        public JenkinsServerManagerSingletonFactory(IWebRequestFactory webRequestFactory, IJenkinsApiFactory jenkinsApiFactory, IDateTimeService dateTimeService)
        {
            _webRequestFactory = webRequestFactory;
            _jenkinsApiFactory = jenkinsApiFactory;
            _dateTimeService = dateTimeService;
        }

        public IJenkinsServerManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new JenkinsServerManager(_webRequestFactory, _jenkinsApiFactory, _dateTimeService);
            }
            return _instance;
        }

    }
}
