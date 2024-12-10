using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public class PushGatewayFixture : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage("prom/pushgateway")
        .WithPortBinding(9091, true)
        .Build();

    public string GetEndpoint() => $"{Uri.UriSchemeHttp}://{_container.Hostname}:{_container.GetMappedPublicPort(9091)}";

    public Task InitializeAsync()
        => _container.StartAsync();

    public Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
}
