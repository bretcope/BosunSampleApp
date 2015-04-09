using System;
using System.Collections.Generic;
using BosunReporter;

namespace BosunSampleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var collector = SetupCollector();
            SampleRunner.RecordMetricsForever(collector);
        }

        private static MetricsCollector SetupCollector()
        {
            var options = new BosunOptions()
            {
                MetricsNamePrefix = "present.",
                GetBosunUrl = () => new Uri("http://192.168.59.103:8070/"),
                ThrowOnPostFail = true,
                ReportingInterval = 5,
                PropertyToTagName = tag => tag.ToLowerInvariant(),
                DefaultTags = new Dictionary<string, string> { { "host", NameTransformers.Sanitize(Environment.MachineName.ToLower()) } }
            };

            return new MetricsCollector(options);
        }
    }
}
