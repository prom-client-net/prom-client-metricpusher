using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher;

/// <summary>
/// Interface for pushing metrics to Prometheus Pushgateway.
/// </summary>
public interface IMetricPusher
{
    /// <summary>
    /// Push metrics to the configured Pushgateway endpoint.
    /// </summary>
    Task PushAsync();
}
