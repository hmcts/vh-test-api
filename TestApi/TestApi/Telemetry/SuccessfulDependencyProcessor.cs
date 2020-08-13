using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace TestApi.Telemetry
{
    public class SuccessfulDependencyProcessor : ITelemetryProcessor
    {
        public SuccessfulDependencyProcessor(ITelemetryProcessor next)
        {
            Next = next;
        }

        private ITelemetryProcessor Next { get; }

        public void Process(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;

            if (dependency?.ResultCode.Equals("404", StringComparison.OrdinalIgnoreCase) == true)
                // To filter out external 404 errors.
                return;

            Next.Process(item);
        }
    }
}