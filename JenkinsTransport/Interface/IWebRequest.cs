using System.Net;

namespace JenkinsTransport.Interface
{
    public interface IWebRequest
    {
        WebHeaderCollection Headers { get; set; }
        string Method { get; set; }
        WebResponse GetResponse();
    }
}