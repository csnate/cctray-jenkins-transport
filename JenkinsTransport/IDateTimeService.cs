using System;

namespace JenkinsTransport
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
    }
}