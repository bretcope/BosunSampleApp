using System;
using System.Threading;
using BosunReporter;
using BosunReporter.Metrics;

namespace BosunSampleApp
{
    public static class SampleRunner
    {
        public static void RecordMetricsForever(MetricsCollector collector)
        {
            var rand = new Random();

            // basic counter (no extra tags)
            var myCounter = collector.CreateMetric<Counter>("my_counter", "units", "desc");

            // counter with tags
//            var successCounter = collector.CreateMetric("results", "units", "desc", new ResultCounter(ResultType.Success));

            // metric group
            var group = collector.GetMetricGroup<ResultType, ResultCounter>("results", "u", "d");
            group.PopulateFromEnum();

            // multi-aggregate gauge
            var gauge = collector.CreateMetric<RandomGauge>("random", "stuff", "no desc");

            // record on them
            while (true)
            {
                myCounter.Increment();

                var x = rand.NextDouble();
                if (x < .1)
                    group[ResultType.Error].Increment();
                else if (x < .3)
                    group[ResultType.Timeout].Increment();
                else
                    group[ResultType.Success].Increment();

                gauge.Record(rand.NextDouble());

                // slow down the infinite loop
                Thread.Sleep(20);
            }
        }
    }

    // Metric types

    public enum ResultType
    {
        Success,
        Error,
        Timeout,
    }

    public class ResultCounter : Counter
    {
        [BosunTag]
        public readonly string Result;

        public ResultCounter(ResultType result)
        {
            Result = result.ToString();
        }
    }

    [GaugeAggregator(AggregateMode.Max)]
    [GaugeAggregator(AggregateMode.Percentile, 0.95)]
    [GaugeAggregator(AggregateMode.Median)]
    [GaugeAggregator(AggregateMode.Min)]
    public class RandomGauge : AggregateGauge
    {
    }
}