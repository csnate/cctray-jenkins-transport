using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsTransport
{
    public class Api
    {
        #region Constants
        private const string AllJobs = "/api/xml?xpath=/hudson/job&wrapper=jobs";
        #endregion

        protected string BaseUrl { get; set; }
        protected string AuthInfo { get; set; }

        public Api(string baseUrl, string authInfo)
        {
            BaseUrl = baseUrl;
            AuthInfo = authInfo;
        }

        /// <summary>
        /// Retrieve all jobs
        /// </summary>
        public List<JenkinsJob> GetAllJobs()
        {
            var xDoc = XmlUtils.GetXDocumentFromUrl(BaseUrl + AllJobs, AuthInfo);
            var list = xDoc.Descendants("job").Select(a => new JenkinsJob(a)).ToList();
            return list;
        }
    }
}
