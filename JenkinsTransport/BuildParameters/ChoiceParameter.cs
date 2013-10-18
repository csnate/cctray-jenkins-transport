using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JenkinsTransport.Interfaces;

namespace JenkinsTransport.BuildParameters
{
    /// <summary>
    /// Choice Build parameter.  Allows selection of a build parameter from multiple options
    /// </summary>
    public class ChoiceParameter : BaseBuildParameter
    {
        public List<string> Options { get; private set; } 

        public ChoiceParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.ChoiceParameterDefinition;

            Options = document.Elements("choice").Select(a => (string)a).ToList();
        }
    }
}
