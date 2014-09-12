﻿using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    /// <summary>
    /// Implementation of a WebResponse for the purpose of unit testing
    /// </summary>
    internal class TestWebResponse : WebResponse, IDisposable
    {
        //readonly Stream responseStream;
        readonly MemoryStream _responseStream;

        /// <summary>Initializes a new instance of <see cref="TestWebResponse"/>
        /// with the response stream to return.</summary>
        public TestWebResponse(Stream responseStream)
        {
            using (responseStream)
            {
                using (var rd = new StreamReader(responseStream))
                {
                    _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rd.ReadToEnd()));
                }
            }
        }
        /// <summary>
        /// Initialize a new instance of TestWebResponse using the supplied
        /// XDocument
        /// </summary>
        /// <param name="xdoc"></param>
        public TestWebResponse(XDocument xdoc)
        {
            //using (var rd = new StreamReader(xdoc.ToString()))
            //{
            //    _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(rd.ReadToEnd()));
            //}
            _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xdoc.ToString()));
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream()
        {
            return _responseStream;
        }

        public new void Dispose()
        {
            base.Dispose();
            _responseStream.Close();
        }
    }
}