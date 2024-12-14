using System;

namespace Prometheus.Client.MetricPusher.Tests;

public class TestableMetricPushServer(IMetricPusher pusher) : MetricPushServer(pusher)
{
    public bool ErrorHandled { get; private set; }

    protected override void OnPushError(IMetricPusher metricPusher, Exception exception)
    {
        ErrorHandled = true;
    }
}
