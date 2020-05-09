using System;
using System.Net;

namespace AvailabilityChecker.AvailabilityCheck
{
    public class ServiceAvailabilityCheckResult
    {
        public DateTime RequestStartDate { get; set; }
        public TimeSpan RequestDuration { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}