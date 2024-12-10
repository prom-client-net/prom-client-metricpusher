using System.Collections.Generic;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Xunit;
using Xunit.Abstractions;

namespace Prometheus.Client.MetricPusher.Tests;

public class MetricPusherTests(PushGatewayFixture fixture, ITestOutputHelper output) : IClassFixture<PushGatewayFixture>
{
    private readonly string _endpoint = fixture.GetEndpoint();
    private readonly IMetricFactory _metricFactory = new MetricFactory(new CollectorRegistry());

    [Fact]
    public async Task Push()
    {
        var counter = _metricFactory.CreateCounter("test_c12", "help");
        counter.Inc();

        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = _endpoint, Job = "pushgateway-test", Instance = "instance" });
        await pusher.PushAsync();
    }

    [Fact]
    public async Task PushWithAdditionalHeaders()
    {
        var counter = _metricFactory.CreateCounter("test_", "help");
        counter.Inc();

        const string accessToken = "";
        var pusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = _endpoint,
            Job = "pushgateway-test",
            Instance = "instance",
            AdditionalHeaders = new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } }
        });
        await pusher.PushAsync();
    }

    [Fact]
    public async Task Worker10Steps()
    {
        var counter = _metricFactory.CreateCounter("worker_counter1", "help");
        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = _endpoint, Job = "pushgateway-testworker" });

        var worker = new MetricPushServer(pusher);
        worker.Start();

        for (int i = 0; i < 10; i++)
        {
            counter.Inc();
            output.WriteLine($"Step: {i}, IsRunning: {worker.IsRunning}");

            switch (i)
            {
                case 5:
                    worker.Stop();
                    break;
                case 8:
                    worker.Start();
                    break;
            }

            await Task.Delay(100);
        }

        worker.Stop();
    }
}
