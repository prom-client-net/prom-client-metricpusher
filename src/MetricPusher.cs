using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ICollectorRegistry _collectorRegistry;
        private readonly Uri _targetUri;

        public MetricPusher(string endpoint, string job)
            : this(endpoint, job, instance: null)
        {
        }

        public MetricPusher(string endpoint, string job, string instance)
            : this(endpoint, job, instance, null, null)
        {
        }

        public MetricPusher(string endpoint, string job, Dictionary<string, string> additionalHeaders)
            : this(endpoint, job, null, null, additionalHeaders)
        {
        }

        public MetricPusher(string endpoint, string job,IEnumerable<KeyValuePair<string, string>> labels)
            : this(endpoint, job, null, labels, null)
        {
        }

        public MetricPusher(string endpoint, string job, string instance, Dictionary<string, string> additionalHeaders)
            : this(endpoint, job, instance, null, additionalHeaders)
        {
        }

        public MetricPusher(string endpoint, string job, string instance, IEnumerable<KeyValuePair<string, string>> labels)
            : this(endpoint, job, instance, labels, null)
        {
        }

        public MetricPusher(
            string endpoint,
            string job,
            string instance,
            IEnumerable<KeyValuePair<string, string>> labels,
            Dictionary<string, string> additionalHeaders)
            : this(null, endpoint, job, instance, labels, additionalHeaders, null)
        {
        }

        public MetricPusher(
            ICollectorRegistry collectorRegistry,
            string endpoint,
            string job,
            string instance,
            IEnumerable<KeyValuePair<string, string>> labels,
            Dictionary<string, string> additionalHeaders,
            HttpClient httpClient)
        {
            if (string.IsNullOrEmpty(job))
                throw new ArgumentNullException(nameof(job));

            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException(nameof(endpoint));

            var stringBuilder = new StringBuilder(endpoint);
            if (!endpoint.EndsWith("/"))
                stringBuilder.Append("/");

            stringBuilder
                .Append("metrics/job/")
                .Append(job);

            if (!string.IsNullOrEmpty(instance))
            {
                stringBuilder
                    .Append("/instance/")
                    .Append(instance);
            }

            if (labels != null)
            {
                foreach (var pair in labels.Where(l => !string.IsNullOrEmpty(l.Key) && !string.IsNullOrEmpty(l.Value)))
                {
                    stringBuilder
                        .Append("/")
                        .Append(pair.Key)
                        .Append("/")
                        .Append(pair.Value);
                }
            }

            if (!Uri.TryCreate(stringBuilder.ToString(), UriKind.Absolute, out _targetUri))
                throw new ArgumentException("Endpoint must be a valid url", nameof(endpoint));

            _collectorRegistry = collectorRegistry ?? Metrics.DefaultCollectorRegistry;

            _httpClient = httpClient ?? new HttpClient();
            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    _httpClient.DefaultRequestHeaders.Add(
                        header.Key,
                        header.Value
                    );
                }
            }
        }

        /// <inheritdoc />
        public async Task PushAsync()
        {
            var memoryStream = await ScrapeHandler.ProcessAsync(_collectorRegistry);
            var response = await _httpClient.PostAsync(_targetUri, new StreamContent(memoryStream));
            response.EnsureSuccessStatusCode();
            memoryStream.Dispose();
        }
    }
}
