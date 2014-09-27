using System;

namespace JenkinsTransport
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}