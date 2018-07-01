using System;
using System.IO;
using System.Net.Http;
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

        /// <summary>
        ///     Constructor
        /// </summary>
        public MetricPusher(string endpoint, string job, string instance)
            : this(null, endpoint, job, instance)
        {
            
        }
        
        /// <summary>
        ///     Constructor
        /// </summary>
        public MetricPusher(ICollectorRegistry collectorRegistry, string endpoint, string job, string instance)
        {
            if (string.IsNullOrEmpty(job))
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));
            
            var url = $"{endpoint.TrimEnd('/')}/metrics/job/{job}";
            if (!string.IsNullOrEmpty(instance))
                url = $"{url}/instance/{instance}";

            if (!Uri.TryCreate(url, UriKind.Absolute, out _targetUri))
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