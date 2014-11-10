using System;
using System.Windows.Forms;

namespace JenkinsTransport.Interface
{
    public interface IForm : IDisposable
    {
        DialogResult ShowDialog(IWin32Window owner);
        string GetServer();
        string GetUsername();
        string GetPassword();
    }
}