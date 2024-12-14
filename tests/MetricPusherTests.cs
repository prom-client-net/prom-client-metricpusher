using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public class MetricPusherTests
{
    private const string _job = "test-job";
    private const string _endpoint = "https://example.org";
    private const string _instance = "test-instance";

    private readonly Dictionary<string, string> _additionalLabels = new()
    {
        { "label1", "value1" },
        { "label2", "value2" }
    };

    [Fact]
    public void Constructor_WhenJobIsNull_ThrowsArgumentNullException()
    {
        var options = new MetricPusherOptions
        {
            Job = null,
            Endpoint = _endpoint
        };

        var ex = Assert.Throws<ArgumentNullException>(() => new MetricPusher(options));
        Assert.Equal(nameof(MetricPusherOptions.Job), ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenEndpointIsNull_ThrowsArgumentNullException()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = null
        };

        var ex = Assert.Throws<ArgumentNullException>(() => new MetricPusher(options));
        Assert.Equal(nameof(MetricPusherOptions.Endpoint), ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenJobIsEmpty_ThrowsArgumentNullException()
    {
        var options = new MetricPusherOptions
        {
            Job = string.Empty,
            Endpoint = _endpoint
        };

        var ex = Assert.Throws<ArgumentNullException>(() => new MetricPusher(options));
        Assert.Equal(nameof(MetricPusherOptions.Job), ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenEndpointIsEmpty_ThrowsArgumentNullException()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = string.Empty
        };

        var ex = Assert.Throws<ArgumentNullException>(() => new MetricPusher(options));
        Assert.Equal(nameof(MetricPusherOptions.Endpoint), ex.ParamName);
    }

    [Fact]
    public void Constructor_WhenEndpointDoesNotEndWithSlash_TargetUriAsExpected()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint
        };

        var pusher = new MetricPusher(options);

        Assert.Equal(new Uri($"{_endpoint}/metrics/job/{_job}"), pusher.TargetUri);
    }

    [Fact]
    public void Constructor_WhenEndpointEndsWithSlash_TargetUriAsExpected()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = $"{_endpoint}/"
        };

        var pusher = new MetricPusher(options);

        Assert.Equal(new Uri($"{_endpoint}/metrics/job/{_job}"), pusher.TargetUri);
    }

    [Fact]
    public void Constructor_WhenEndpointIsInvalid_ThrowsArgumentException()
    {
        const string invalidEndpoint = "invalid-url";

        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = invalidEndpoint
        };

        var ex = Assert.Throws<ArgumentException>(() => new MetricPusher(options));

        Assert.Contains("Endpoint must be a valid url", ex.Message);
    }

    [Fact]
    public void Constructor_WhenInstanceIsProvided_TargetUriAsExpected()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            Instance = _instance
        };

        var pusher = new MetricPusher(options);

        Assert.Equal(new Uri($"{_endpoint}/metrics/job/{_job}/instance/{_instance}"), pusher.TargetUri);
    }

    [Fact]
    public void Constructor_WhenAdditionalLabelsAreProvided_TargetUriAsExpected()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            AdditionalLabels = _additionalLabels
        };

        var pusher = new MetricPusher(options);

        Assert.Equal(new Uri($"{_endpoint}/metrics/job/{_job}/label1/value1/label2/value2"), pusher.TargetUri);
    }

    [Fact]
    public void Constructor_WhenInstanceAndLabelsAreProvided_TargetUriAsExpected()
    {
        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            Instance = _instance,
            AdditionalLabels = _additionalLabels
        };

        var pusher = new MetricPusher(options);

        Assert.Equal(
            new Uri($"{_endpoint}/metrics/job/{_job}/instance/{_instance}/label1/value1/label2/value2"),
            pusher.TargetUri
        );
    }

    [Fact]
    public async Task Push_WhenCalled_SendsMetricsToTargetUri()
    {
        var collectorRegistry = Substitute.For<ICollectorRegistry>();
        var mockHandler = new MockHttpMessageHandler();

        ScrapeHandler.ProcessAsync(collectorRegistry).Returns(Task.FromResult(new MemoryStream()));

        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            CollectorRegistry = collectorRegistry,
            HttpClient = new HttpClient(mockHandler)
        };

        var pusher = new MetricPusher(options);

        await pusher.PushAsync();

        Assert.Equal(HttpMethod.Post, mockHandler.RequestMethod);
        Assert.Equal(pusher.TargetUri, mockHandler.RequestUri);
        Assert.NotNull(mockHandler.Content);
    }

    [Fact]
    public async Task Push_WhenHttpResponseFails_ThrowsException()
    {
        var collectorRegistry = Substitute.For<ICollectorRegistry>();
        var mockHandler = new MockHttpMessageHandler(HttpStatusCode.InternalServerError);

        ScrapeHandler.ProcessAsync(collectorRegistry).Returns(Task.FromResult(new MemoryStream()));

        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            CollectorRegistry = collectorRegistry,
            HttpClient = new HttpClient(mockHandler)
        };

        var pusher = new MetricPusher(options);

        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => pusher.PushAsync());

        Assert.Contains("500", ex.Message);
    }


    [Fact]
    public async Task Dispose_WhenHttpClientIsProvided_DoesNotDisposeHttpClient()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler();
        var httpClient = new HttpClient(mockHandler);

        var options = new MetricPusherOptions
        {
            Job = _job,
            Endpoint = _endpoint,
            HttpClient = httpClient // Simulate provided HttpClient
        };

        var pusher = new MetricPusher(options);

        pusher.Dispose();

        // HttpClient should still work after Dispose is called
        var response = await httpClient.GetAsync(_endpoint);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
