using System;
using System.Runtime.InteropServices;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests;

public sealed class IntegrationFactAttribute : FactAttribute
{
    public IntegrationFactAttribute()
    {
        bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var isGitHubActions);

        if (isGitHubActions && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Skip = "Integration test skipped due to environment conditions";
        }
    }
}
