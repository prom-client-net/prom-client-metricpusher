using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using Prometheus.Client.Collectors;
using Xunit;
using Xunit.Abstractions;

namespace Prometheus.Client.MetricPusher.Tests.Integration;
// Need Environment
// https://github.com/prometheus/pushgateway

public class MetricPusherTests
{
    private readonly ITestOutputHelper _output;
    private readonly IMetricFactory _metricFactory;

    public MetricPusherTests(ITestOutputHelper output)
    {
        _output = output;
        _metricFactory = new MetricFactory(new CollectorRegistry());
    }

    [Fact]
    public async Task Simple_Push()
    {
        var counter = _metricFactory.CreateCounter("test_c12", "help");
        counter.Inc();

        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = "http://localhost:9091", Job = "pushgateway-test", Instance = "instance" });
        await pusher.PushAsync();
    }

    [Fact]
    public async Task Auth_Puth()
    {
        var counter = _metricFactory.CreateCounter("test_", "help");
        counter.Inc();

        const string accessToken = "";
        var pusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = "http://localhost:9091",
            Job = "pushgateway-test",
            Instance = "instance",
            AdditionalHeaders = new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } }
        });
        await pusher.PushAsync();
    }

    [Fact]
    public async Task Worker_10Step()
    {
        var counter = _metricFactory.CreateCounter("worker_counter1", "help");
        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = "http://localhost:9091", Job = "pushgateway-testworker" });

        var worker = new MetricPushServer(pusher);
        worker.Start();

        for (int i = 0; i < 10; i++)
        {
            counter.Inc();
            _output.WriteLine($"Step: {i}, IsRunning: {worker.IsRunning}");

            switch (i)
            {
                case 5:
                    worker.Stop();
                    break;
                case 8:
                    worker.Start();
                    break;
            }

            await Task.Delay(2000);
        }

        worker.Stop();
    }

    [Fact]
    public async Task TestPushContinuesOnError()
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
