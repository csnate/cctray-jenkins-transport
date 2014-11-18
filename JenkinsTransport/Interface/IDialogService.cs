namespace JenkinsTransport.Interface
{
    public interface IDialogService
    {
        void Show(string text, string caption);
        void Show(string text);
    }
}