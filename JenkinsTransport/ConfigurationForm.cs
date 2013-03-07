using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JenkinsTransport
{
    public partial class ConfigurationForm : Form
    {
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
    }
}
