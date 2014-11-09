using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JenkinsTransport
{
    public class ConfigurationFormFactory : IFormFactory
    {
        public IForm Create()
        {
            return new ConfigurationForm();
        }
    }

    public interface IFormFactory
    {
        IForm Create();
    }
}
