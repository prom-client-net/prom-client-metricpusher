using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
    public async Task PushWithoutException()
    {
        var counter = _metricFactory.CreateCounter("counter1", "help");
        counter.Inc();

        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = _endpoint, Job = "pushgateway-test", Instance = "instance" });

        var ex = await Record.ExceptionAsync(() => pusher.PushAsync());
        Assert.Null(ex);
    }

    [Fact]
    public async Task PushToWrongUrlWithHttpRequestException()
    {
        var counter = _metricFactory.CreateCounter("counter2", "help");
        counter.Inc();

        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = "http://localhost:2543", Job = "pushgateway-test", Instance = "instance" });

        var ex = await Record.ExceptionAsync(() => pusher.PushAsync());
        Assert.IsType<HttpRequestException>(ex);
    }

    [Fact]
    public async Task PushWithAdditionalHeadersWithoutException()
    {
        var counter = _metricFactory.CreateCounter("counter3", "help");
        counter.Inc();

        const string accessToken = "";
        var pusher = new MetricPusher(new MetricPusherOptions
        {
            Endpoint = _endpoint,
            Job = "pushgateway-test",
            Instance = "instance",
            AdditionalHeaders = new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } }
        });

        var ex = await Record.ExceptionAsync(() => pusher.PushAsync());
        Assert.Null(ex);
    }

    [Fact]
    public async Task Worker10StepsWithExpectedResult()
    {
        var counter = _metricFactory.CreateCounter("worker_counter1", "help");
        var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = _endpoint, Job = "pushgateway-testworker" });
        var worker = new MetricPushServer(pusher);

        var resultBuilder = new StringBuilder();
        const string expectedResult = """
                                      Step: 0, IsRunning: True
                                      Step: 1, IsRunning: True
                                      Step: 2, IsRunning: True
                                      Step: 3, IsRunning: True
                                      Step: 4, IsRunning: True
                                      Step: 5, IsRunning: True
                                      Step: 6, IsRunning: False
                                      Step: 7, IsRunning: False
                                      Step: 8, IsRunning: False
                                      Step: 9, IsRunning: True

                                      """;

        worker.Start();

        for (int i = 0; i < 10; i++)
        {
            var info = $"Step: {i}, IsRunning: {worker.IsRunning}";
            resultBuilder.AppendLine(info);
            output.WriteLine(info);

            counter.Inc();

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

        Assert.Equal(expectedResult, resultBuilder.ToString());

        worker.Stop();
    }
}
