using System;
using System.Threading.Tasks;
using NSubstitute;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public class MetricPushServerTests
{
    [Fact]
    public async Task Push_WithDefaultInterval_InvokedExpectedTimes()
    {
        var pusher = Substitute.For<IMetricPusher>();

        var worker = new MetricPushServer(pusher);
        worker.Start();
        await Task.Delay(2500);

        await pusher.Received(3).PushAsync();
        worker.Stop();
    }

    [Fact]
    public async Task Push_WithCustomInterval_ContinuesOnError_InvokedExpectedTimes()
    {
        var pusher = Substitute.For<IMetricPusher>();

        var worker = new MetricPushServer(pusher, TimeSpan.FromMilliseconds(2000));
        worker.Start();
        await Task.Delay(3500);

        await pusher.Received(2).PushAsync();
        worker.Stop();
    }

    [Fact]
    public void Worker_StartStop_IsRunningAsExpected()
    {
        var pusher = Substitute.For<IMetricPusher>();
        var worker = new MetricPushServer(pusher);

        Assert.False(worker.IsRunning);
        worker.Start();
        Assert.True(worker.IsRunning);
        worker.Stop();
        Assert.False(worker.IsRunning);
    }

    [Fact]
    public async Task Worker_WhenPushThrowsException_DoesNotInterruptWork()
    {
        var pusher = Substitute.For<IMetricPusher>();
        pusher.PushAsync().Returns(Task.FromException(new Exception("Simulated Push Exception")));

        var worker = new MetricPushServer(pusher);
        worker.Start();
        await Task.Delay(1500);

        await pusher.Received(2).PushAsync();
        worker.Stop();
    }

    [Fact]
    public async Task Worker_OnPushError_HandlesException()
    {
        var pusher = Substitute.For<IMetricPusher>();
        pusher.PushAsync().Returns(Task.FromException(new Exception("Simulated Push Exception")));

        var worker = new TestableMetricPushServer(pusher);
        worker.Start();
        await Task.Delay(1000);

        Assert.True(worker.ErrorHandled);
        worker.Stop();
    }

    [Fact]
    public async Task Worker_PushesFinalMetricsOnStop()
    {
        var pusher = Substitute.For<IMetricPusher>();

        var worker = new MetricPushServer(pusher);
        worker.Start();
        await Task.Delay(1500);

        worker.Stop();
        await Task.Delay(100);

        await pusher.Received(3).PushAsync(); // 2 + 1 (final push)
    }
}
