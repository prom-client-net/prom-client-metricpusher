using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;

namespace Prometheus.Client.MetricPusher
{
    /// <inheritdoc />
    public class MetricPusher : IMetricPusher
    {
        private readonly HttpClient _httpClient;
        private const string _contentType = "text/plain; version=0.0.4";
        private readonly ICollectorRegistry _collectorRegistry;
        private readonly Uri _targetUri;

        public MetricPusher(string endpoint, string job)
            : this(endpoint, job, null)
        {
        }
        
        public MetricPusher(string endpoint, string job, string instance)
            : this(null, endpoint, job, instance)
        {
        }

        public MetricPusher(ICollectorRegistry collectorRegistry, string endpoint, string job, string instance)
        {
            if (string.IsNullOrEmpty(job))
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            var stringBuilder = new StringBuilder(endpoint.TrimEnd('/') + "/metrics/job/" + job);
            if (!string.IsNullOrEmpty(instance))
                stringBuilder.Append("/instance/" + instance);

            if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out _targetUri))
                throw new ArgumentException("Endpoint must be a valid url", nameof(endpoint));

            _collectorRegistry = collectorRegistry ?? CollectorRegistry.Instance;
            _httpClient = new HttpClient();
        }

        /// <inheritdoc />
        public async Task PushAsync()
        {
            using (var memoryStream = new MemoryStream())
            {
                var metrics = _collectorRegistry.CollectAll();
                ScrapeHandler.ProcessScrapeRequest(metrics, _contentType, memoryStream);
                memoryStream.Position = 0;
                var response = await _httpClient.PostAsync(_targetUri, new StreamContent(memoryStream));
                response.EnsureSuccessStatusCode();
            }
        }
    }
}