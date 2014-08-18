using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace JenkinsTransport
{
    public class ApiNonStatic : Api
    {
        public IWebRequestFactory WebRequestFactory;

        public ApiNonStatic(string baseUrl, string authInfo) : base(baseUrl, authInfo)
        {
        }

        protected new XDocument GetXDocument(string url, string authInfo)
        {
            var request = WebRequestFactory.Create(url);

            if (!String.IsNullOrEmpty(authInfo))
            {
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
            request.Method = "GET";

            return GetXDocument(request);
        }

        protected XDocument GetXDocument(IWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var rd = new StreamReader(responseStream))
                        {
                            return XDocument.Parse(rd.ReadToEnd());
                        }
                    }
                }
            }
            return new XDocument();
        }

        public XDocument GetDoc()
        {
            return GetXDocument(BaseUrl + AllJobs, AuthInfo);            
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        public new List<JenkinsJob> GetAllJobs()
        {
            var xDoc = GetXDocument(BaseUrl + AllJobs, AuthInfo);
            return GetAllJobs(xDoc);
        }
    }
}
