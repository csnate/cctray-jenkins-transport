using JenkinsTransport.Interface;
using System.Windows.Forms;

namespace JenkinsTransport
{
    /// <summary>
    /// Implementation of a DialogService that calls through to the windows forms MessageBox
    /// </summary>
    public class MessageBoxService : IDialogService
    {
        public void Show(string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        public void Show(string text)
        {
            MessageBox.Show(text);
        }
    }
}
