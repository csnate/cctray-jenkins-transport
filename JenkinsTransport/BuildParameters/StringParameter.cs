using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JenkinsTransport.Interfaces;

namespace JenkinsTransport.BuildParameters
{
    public class StringParameter : BaseBuildParameter
    {
        public StringParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.StringParameterDefinition;
        }
    }
}
