using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JenkinsTransport.Interfaces;

namespace JenkinsTransport.BuildParameters
{
    public class BaseBuildParameter : IBuildParameter
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public BuildParameterType ParameterType { get; protected set; }
        public string DefaultValue { get; private set; }

        public BaseBuildParameter(XContainer document)
        {
            Name = (string)document.Element("name");
            Description = (string)document.Element("description");
            DefaultValue = (string)document.Descendants("defaultParameterValue").First().Element("value");
        }
    }
}
