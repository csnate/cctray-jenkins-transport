using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public interface IJenkinsServerManagerFactory
    {
        IJenkinsServerManager GetInstance();
        bool IsServerManagerInitialized { get; set; }
    }
}