using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JenkinsTransport.Interfaces;

namespace JenkinsTransport.BuildParameters
{
    public class BooleanParameter : BaseBuildParameter
    {
        public BooleanParameter(XContainer document) : base(document)
        {
            ParameterType = BuildParameterType.BooleanParameterDefinition;
        }
    }
}
