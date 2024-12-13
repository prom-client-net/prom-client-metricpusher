using System;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public class MetricPushServerTests
{
    [Fact]
    public async Task PushContinuesOnError()
    {
        var pusher = Substitute.For<IMetricPusher>();
        pusher.PushAsync().Returns(Task.FromException(new Exception("Simulated Push Error")));

        var worker = new MetricPushServer(pusher);
        worker.Start();
        await Task.Delay(2500);

        await pusher.Received(3).PushAsync();
        worker.Stop();
    }
}
