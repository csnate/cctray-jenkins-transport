using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace JenkinsTransport
{
    public class XmlUtils
    {
        /// <summary>
        /// Returns an XmlDocument from the requested URL.
        /// </summary>
        /// <param name="url">the url to request</param>
        /// <param name="authInfo">the autho info to set in the header</param>
        /// <returns></returns>
        public static XmlDocument GetXmlDocumentFromUrl(string url, string authInfo)
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

        /// <summary>
        /// Returns an XDocument from the requested URL.
        /// </summary>
        /// <param name="url">the url to request</param>
        /// <param name="authInfo">the autho info to set in the header</param>
        /// <returns></returns>
        public static XDocument GetXDocumentFromUrl(string url, string authInfo)
        {
            var request = WebRequest.Create(url);
            if (!String.IsNullOrEmpty(authInfo))
            {
                request.Headers["Authorization"] = "Basic " + authInfo;
            }
            request.Method = "GET";

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

        /// <summary>
        /// Convert an XML node to a specific type
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="node">the xmlNode to convert</param>
        public static T ConvertNode<T>(XmlNode node) where T : class
        {
            using (var memStream = new MemoryStream())
            {
                using (var stw = new StreamWriter(memStream))
                {
                    stw.Write(node.OuterXml);
                    stw.Flush();
                    memStream.Position = 0;

                    var ser = new XmlSerializer(typeof (T));
                    var result = (ser.Deserialize(memStream) as T);

                    return result;
                }
            }
        }
    }
}
