using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JenkinsTransport.BuildParameters;

namespace JenkinsTransport.Interfaces
{
    public interface IBuildParameter
    {
        string Name { get; }
        string Description { get; }
        BuildParameterType ParameterType { get; }
        string DefaultValue { get; }
    }
}
