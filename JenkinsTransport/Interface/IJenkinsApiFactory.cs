namespace JenkinsTransport.Interface
{
    public interface IJenkinsApiFactory
    {        
        IJenkinsApi Create(string url, string authorizationInformation, IWebRequestFactory webRequestFactory);
    }
}
