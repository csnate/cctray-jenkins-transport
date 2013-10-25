using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.BuildParameters
{
    public class StringParameter : BaseBuildParameter
    {
        public StringParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.StringParameterDefinition;
        }

        public override ParameterBase ToParameterBase()
        {
            return new TextParameter()
                   {
                       Name = Name,
                       Description = Description,
                       DefaultValue = DefaultValue,
                       IsRequired = true,
                       DisplayName = Name
                   };
        }
    }
}
