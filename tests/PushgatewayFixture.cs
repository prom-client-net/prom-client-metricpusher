using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public class PushgatewayFixture : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder("prom/pushgateway")
        .WithPortBinding(9091, true)
        .Build();

    public string GetEndpoint() => $"{Uri.UriSchemeHttp}://{_container.Hostname}:{_container.GetMappedPublicPort(9091)}";

#if NET462
    public Task InitializeAsync()
        => _container.StartAsync();

    public Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
#else
    public async ValueTask InitializeAsync()
        => await _container.StartAsync();

    public async ValueTask DisposeAsync()
        => await _container.DisposeAsync();
#endif
}
