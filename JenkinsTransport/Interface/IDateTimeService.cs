using System;

namespace JenkinsTransport.Interface
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
    }
}