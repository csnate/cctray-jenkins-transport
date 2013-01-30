using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace JenkinsTransport
{
    public class XmlUtils
    {
        /// <summary>
        /// Returns an XmlDocument from the requested URL.
        /// This is the base method that most methods will start
        /// </summary>
        /// <param name="url">the url to request</param>
        /// <param name="authInfo">the autho info to set in the header</param>
        /// <returns></returns>
        public static XmlDocument GetXmlFromUrl(string url, string authInfo)
        {
            var request = WebRequest.Create(url);
            if (!String.IsNullOrEmpty(authInfo))
            {
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
            request.Method = "GET";

            var xmlDoc = new XmlDocument();
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var rd = new StreamReader(responseStream))
                        {
                            xmlDoc.Load(rd);
                        }
                    }
                }
            }
            return xmlDoc;
        }
    }
}
