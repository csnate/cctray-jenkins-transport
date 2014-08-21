using System.IO;
using System.Net;

namespace JenkinsTransport.UnitTests.TestHelpers
{
    /// <summary>
    /// Implementation of a WebResponse for the purpose of unit testing
    /// </summary>
    internal class TestWebReponse : WebResponse
    {
        Stream responseStream;

        /// <summary>Initializes a new instance of <see cref="TestWebReponse"/>
        /// with the response stream to return.</summary>
        public TestWebReponse(Stream responseStream)
        {
            this.responseStream = responseStream;
        }

        /// <summary>See <see cref="WebResponse.GetResponseStream"/>.</summary>
        public override Stream GetResponseStream()
        {
            return responseStream;
        }
    }
}