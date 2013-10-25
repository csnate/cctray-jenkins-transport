namespace JenkinsTransport.BuildParameters
{
    /// <summary>
    /// Enum of all available Jenkins Build Parameter types.
    /// </summary>
    /// <remarks>
    /// The name here should identically match the value of the "type" node in the "parameterDefinition" node for the build.
    /// </remarks>
    public enum BuildParameterType
    {
        BooleanParameterDefinition,
        ChoiceParameterDefinition,
        StringParameterDefinition
    }
}
