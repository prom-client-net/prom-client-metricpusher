using System.Collections.Generic;
using System.Net.Http;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.MetricPusher;

public class MetricPusherOptions
{
    public ICollectorRegistry CollectorRegistry { get; set; }

    public string Endpoint { get; set; }

    public string Job { get; set; }

    public string Instance { get; set; }

    public IEnumerable<KeyValuePair<string, string>> AdditionalLabels { get; set; }

    public IEnumerable<KeyValuePair<string, string>> AdditionalHeaders { get; set; }

    public HttpClient HttpClient { get; set; }
}
