using System.Collections.Generic;
using System.Net.Http;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.MetricPusher;

/// <summary>
/// Configuration options for the metric pusher.
/// </summary>
public class MetricPusherOptions
{
    /// <summary>
    /// The <see cref="ICollectorRegistry"/> instance to use for metric collection.
    /// </summary>
    public ICollectorRegistry CollectorRegistry { get; set; }

    /// <summary>
    /// The Pushgateway endpoint URL.
    /// </summary>
    public string Endpoint { get; set; }

    /// <summary>
    /// The job name for metrics.
    /// </summary>
    public string Job { get; set; }

    /// <summary>
    /// The instance name for metrics.
    /// </summary>
    public string Instance { get; set; }

    /// <summary>
    /// Additional labels to include with metrics.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> AdditionalLabels { get; set; }

    /// <summary>
    /// Additional HTTP headers to include in requests.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> AdditionalHeaders { get; set; }

    /// <summary>
    /// The HTTP client to use for requests.
    /// </summary>
    public HttpClient HttpClient { get; set; }
}
