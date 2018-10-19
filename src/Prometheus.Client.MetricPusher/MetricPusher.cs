using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Prometheus.Client.Collectors.Abstractions;

namespace Prometheus.Client.MetricPusher
{
    /// <inheritdoc />
    public class MetricPusher : IMetricPusher
    {
        private readonly HttpClient _httpClient;
        private readonly ICollectorRegistry _collectorRegistry;
        private readonly Uri _targetUri;
               
        public MetricPusher(string endpoint, string job, string instance = null, IEnumerable<KeyValuePair<string, string>> labels = null)
            : this(null, endpoint, job, instance, labels)
        {
        }

        public MetricPusher(ICollectorRegistry collectorRegistry, string endpoint, string job, string instance, IEnumerable<KeyValuePair<string, string>> labels)
        {
            if (string.IsNullOrEmpty(job))
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            var stringBuilder = new StringBuilder(endpoint.TrimEnd('/') + "/metrics/job/" + job);
            if (!string.IsNullOrEmpty(instance))
                stringBuilder.Append("/instance/" + instance);
            
            if (labels != null)
            {
                foreach (var pair in labels.Where(l=>!string.IsNullOrEmpty(l.Key) && !string.IsNullOrEmpty(l.Value)))
                {
                    stringBuilder.Append("/" + pair.Key + "/" + pair.Value);
                }
            }

            if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out _targetUri))
                throw new ArgumentException("Endpoint must be a valid url", nameof(endpoint));

            _collectorRegistry = collectorRegistry ?? CollectorRegistry.Instance;
            _httpClient = new HttpClient();
        }

        /// <inheritdoc />
        public async Task PushAsync()
        {
            var memoryStream = ScrapeHandler.Process(_collectorRegistry);
            var response = await _httpClient.PostAsync(_targetUri, new StreamContent(memoryStream));
            response.EnsureSuccessStatusCode();
            memoryStream.Dispose();
        }
    }
}
