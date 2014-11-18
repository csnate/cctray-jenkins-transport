using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JenkinsTransport.Interface;

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
