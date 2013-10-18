using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JenkinsTransport.BuildParameters;
using JenkinsTransport.Interfaces;

namespace JenkinsTransport
{
    public class JenkinsBuildInformation
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1).ToLocalTime();

        public bool Building { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string Number { get; private set; }
        public int Duration { get; private set; }
        public int EstimatedDuration { get; private set; }
        public string FullDisplayName { get; private set; }
        public string Id { get; private set; }
        public List<IBuildParameter> BuildParameters { get; set; }

        public JenkinsBuildInformation()
        {
            Timestamp = DateTime.Now;
            Number = String.Empty;
            Duration = 0;
            EstimatedDuration = 0;
            Id = String.Empty;
            Building = false;
            FullDisplayName = String.Empty;
            BuildParameters = new List<IBuildParameter>();
        }

        public JenkinsBuildInformation(XContainer document)
        {
            var firstElement = document.Descendants().First<XElement>();
            Timestamp = Epoch.AddMilliseconds((long) firstElement.Element("timestamp"));
            Number = (string) firstElement.Element("number");
            Duration = (int) firstElement.Element("duration");
            EstimatedDuration = (int) firstElement.Element("estimatedDuration");
            FullDisplayName = (string) firstElement.Element("fullDisplayName");
            Id = (string) firstElement.Element("id");
            Building = (bool) firstElement.Element("building");

            // Construct the build parameters
            BuildParameters = new List<IBuildParameter>();
            var parameters = firstElement.Descendants("action").Elements("parameterDefinition");
            var supportedTypes = Enum.GetNames(typeof(BuildParameterType));
            foreach (var parameter in parameters)
            {
                var type = (string) parameter.Element("type");
                if (!supportedTypes.Contains(type)) continue;

                switch ((BuildParameterType)Enum.Parse(typeof (BuildParameterType), type))
                {
                    case BuildParameterType.BooleanParameterDefinition:
                        BuildParameters.Add(new BooleanParameter(parameter));
                        break;

                    case BuildParameterType.ChoiceParameterDefinition:
                        BuildParameters.Add(new ChoiceParameter(parameter));
                        break;
                    
                    case BuildParameterType.StringParameterDefinition:
                        BuildParameters.Add(new StringParameter(parameter));
                        break;
                }
            }

        }
    }
}
