using System.IO;
using System.Net;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    /// <summary>
    /// Implementation of WebRequest for the purpose of unit testing
    /// </summary>
    class TestWebRequest : WebRequest
    {
        MemoryStream requestStream = new MemoryStream();
        readonly MemoryStream _responseStream;

        public override string Method { get; set; }
        public override string ContentType { get; set; }
        public override long ContentLength { get; set; }

        /// <summary>Initializes a new instance of <see cref="TestWebRequest"/>
        /// with the response to return.</summary>
        public TestWebRequest(string response)
        {
            _responseStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(response));
        }

        /// <summary>Returns the request contents as a string.</summary>
        public string GetRequestAsString()
        {
            return System.Text.Encoding.UTF8.GetString(requestStream.ToArray());
        }

        /// <summary>See <see cref="WebRequest.GetRequestStream"/>.</summary>
        public override Stream GetRequestStream()
        {
            return requestStream;
        }

        /// <summary>See <see cref="WebRequest.GetResponse"/>.</summary>
        public override WebResponse GetResponse()
        {
            return new TestWebResponse(_responseStream);
        }
    }
}
