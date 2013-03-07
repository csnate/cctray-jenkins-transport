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
        private Regex ServerRegex = new Regex("^http(s)?://\\w+", RegexOptions.IgnoreCase);

        private bool IsValidServer()
        {
            var server = GetServer();
            return ServerRegex.IsMatch(server);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (!IsValidServer())
            {
                e.Cancel = true;
                errorProvider1.SetError(textBox1, "Please provide a valid server");
            }
        }

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        public string GetServer()
        {
            return textBox1.Text;
        }

        public string GetUsername()
        {
            return textBox2.Text;
        }

        public string GetPassword()
        {
            return textBox3.Text;
        }

        private void textBox1_Validated(object sender, EventArgs e)
        {
            errorProvider1.SetError(textBox1, String.Empty);
        }
    }
}
