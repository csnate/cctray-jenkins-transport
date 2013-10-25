using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.BuildParameters
{
    /// <summary>
    /// Choice Build parameter.  Allows selection of a build parameter from multiple options
    /// </summary>
    public class ChoiceParameter : BaseBuildParameter
    {
        public NameValuePair[] Options { get; private set; } 

        public ChoiceParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.ChoiceParameterDefinition;

            Options = document.Elements("choice").Select(a => new NameValuePair(a.Value, a.Value)).ToArray();
        }

        public override ParameterBase ToParameterBase()
        {
            return new SelectParameter()
                   {
                       Name = Name,
                       Description = Description,
                       DefaultValue = DefaultValue,
                       IsRequired = true,
                       DisplayName = Name,
                       DataValues = Options
                   };
        }
    }
}
