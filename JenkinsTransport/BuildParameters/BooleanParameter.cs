using System.Xml.Linq;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace JenkinsTransport.BuildParameters
{
    public class BooleanParameter : BaseBuildParameter
    {
        public BooleanParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.BooleanParameterDefinition;
        }

        public override ParameterBase ToParameterBase()
        {
            return new ThoughtWorks.CruiseControl.Remote.Parameters.BooleanParameter()
            {
                Name = Name,
                FalseValue = new NameValuePair("false", "false"),
                TrueValue = new NameValuePair("true", "true"),
                Description = Description,
                DefaultValue = DefaultValue,
                IsRequired = true,
                DisplayName = Name
            };
        }
    }
}
