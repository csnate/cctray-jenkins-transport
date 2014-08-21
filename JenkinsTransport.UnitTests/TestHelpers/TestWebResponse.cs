using System;
using System.IO;
using System.Net;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    /// <summary>
    /// Implementation of a WebResponse for the purpose of unit testing
    /// </summary>
    internal class TestWebResponse : WebResponse, IDisposable
    {
        //readonly Stream responseStream;
        MemoryStream _responseStream;

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
            
            //this.responseStream =  new MemoryStream();
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