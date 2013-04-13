using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JenkinsTransport.UnitTests
{
    [TestClass]
    public class ConfigurationFormTests
    {
        [TestMethod]
        public void TestGetProperties()
        {
            using (var form = new ConfigurationForm()) {
                var panel = ((FlowLayoutPanel)form.Controls[1]);

                ((TextBox)panel.Controls[1]).Text = "http://server.com";
                ((TextBox)panel.Controls[3]).Text = "user";
                ((TextBox)panel.Controls[5]).Text = "pass";

                Assert.AreEqual(form.GetServer(), "http://server.com");
                Assert.AreEqual(form.GetUsername(), "user");
                Assert.AreEqual(form.GetPassword(), "pass");
            }
        }

        [TestMethod]
        public void TestServerValidation()
        {
            using (var form = new ConfigurationForm())
            {
                var panel = ((FlowLayoutPanel)form.Controls[1]);
                var server = ((TextBox)panel.Controls[1]);
                server.Text = "asd";
                server.Refresh();
                Assert.IsFalse(form.ValidateChildren());

                server.Text = "http://";
                server.Refresh();
                Assert.IsFalse(form.ValidateChildren());

                server.Text = "http://asd.com";
                server.Refresh();
                Assert.IsTrue(form.ValidateChildren());
            }
        }
    }
}
