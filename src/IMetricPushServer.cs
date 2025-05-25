namespace Prometheus.Client.MetricPusher;

/// <summary>
/// Interface for metric push server that periodically pushes metrics.
/// </summary>
public interface IMetricPushServer
{
    /// <summary>
    /// Get a value indicating whether the server is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Start the metric push server.
    /// </summary>
    void Start();

    /// <summary>
    /// Stop the metric push server.
    /// </summary>
    void Stop();
}
