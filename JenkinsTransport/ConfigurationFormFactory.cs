using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JenkinsTransport.Interface;

namespace JenkinsTransport
{
    public class ConfigurationFormFactory : IFormFactory
    {
        public IForm Create()
        {
            return new ConfigurationForm();
        }
    }
}
