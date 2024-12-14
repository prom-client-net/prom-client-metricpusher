using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher.Tests;

public class MockHttpMessageHandler(HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
{
    public HttpMethod RequestMethod { get; private set; }
    public Uri RequestUri { get; private set; }
    public HttpContent Content { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestMethod = request.Method;
        RequestUri = request.RequestUri;
        Content = request.Content;

        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent("Mock Response")
        });
    }
}
