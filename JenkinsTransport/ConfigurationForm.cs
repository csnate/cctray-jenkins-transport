using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JenkinsTransport
{
    public partial class ConfigurationForm : Form
    {
        private static Regex _serverRegex = new Regex("^http(s)?://\\w+", RegexOptions.IgnoreCase);
        private static bool IsValidServer(string server)
        {
            return _serverRegex.IsMatch(server);
        }

        private void ServerTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!IsValidServer(GetServer()))
            {
                e.Cancel = true;
                errorProvider1.SetError(ServerTextBox, "Please provide a valid server URL");
            }
        }

        private void ServerTextBox_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(ServerTextBox, String.Empty);
        }

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the server name from the form
        /// </summary>
        public string GetServer()
        {
            return ServerTextBox.Text;
        }

        /// <summary>
        /// Gets the username from the form
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            return UsernameTextBox.Text;
        }

        /// <summary>
        /// Gets the password from the form
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            return PasswordTextBox.Text;
        }

    }
}
