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
        pusher.PushAsync().Returns(Task.FromException(new Exception("Push error")));

        var worker = new MetricPushServer(pusher, TimeSpan.FromSeconds(0.05));
        worker.Start();
        await Task.Delay(150);

        await pusher.Received(3).PushAsync();
        worker.Stop();
    }
}
