namespace JenkinsTransport.Interface
{
    public interface IJenkinsServerManagerFactory
    {
        IJenkinsServerManager GetInstance();
        bool IsServerManagerInitialized { get; set; }
    }
}