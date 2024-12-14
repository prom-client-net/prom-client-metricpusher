using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.MetricPusher;

public class MetricPusher : IMetricPusher, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _isOwnHttpClient;
    private readonly ICollectorRegistry _collectorRegistry;
    private readonly Uri _targetUri;

    public Uri TargetUri => _targetUri;

    public MetricPusher(MetricPusherOptions options)
    {
        if (string.IsNullOrEmpty(options.Job))
            throw new ArgumentNullException(nameof(options.Job));

        if (string.IsNullOrEmpty(options.Endpoint))
            throw new ArgumentNullException(nameof(options.Endpoint));

        var stringBuilder = new StringBuilder(options.Endpoint);
        if (!options.Endpoint.EndsWith("/"))
            stringBuilder.Append('/');

        stringBuilder
            .Append("metrics/job/")
            .Append(options.Job);

        if (!string.IsNullOrEmpty(options.Instance))
        {
            stringBuilder
                .Append("/instance/")
                .Append(options.Instance);
        }

        if (options.AdditionalLabels != null)
        {
            foreach (var pair in options.AdditionalLabels.Where(l => !string.IsNullOrEmpty(l.Key) && !string.IsNullOrEmpty(l.Value)))
            {
                stringBuilder
                    .Append('/')
                    .Append(pair.Key)
                    .Append('/')
                    .Append(pair.Value);
            }
        }

        if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out _targetUri))
            throw new ArgumentException("Endpoint must be a valid url", nameof(options.Endpoint));

        _collectorRegistry = options.CollectorRegistry ?? Metrics.DefaultCollectorRegistry;

        _httpClient = options.HttpClient ?? new HttpClient();
        _isOwnHttpClient = options.HttpClient == null;
        if (options.AdditionalHeaders != null)
        {
            foreach (var header in options.AdditionalHeaders)
            {
                _httpClient.DefaultRequestHeaders.Add(
                    header.Key,
                    header.Value
                );
            }
        }
    }

    public async Task PushAsync()
    {
        var memoryStream = await ScrapeHandler.ProcessAsync(_collectorRegistry);
        var response = await _httpClient.PostAsync(_targetUri, new StreamContent(memoryStream));
        response.EnsureSuccessStatusCode();
        memoryStream.Dispose();
    }

    public void Dispose()
    {
        if (_isOwnHttpClient)
            _httpClient?.Dispose();
    }
}
