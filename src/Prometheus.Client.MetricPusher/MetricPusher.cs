using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Prometheus.Client.Contracts;

namespace Prometheus.Client.MetricPusher
{
    /// <inheritdoc />
    public class MetricPusher : IMetricPusher
    {
        private HttpClient _httpClient;

        private const string _contentType = "text/plain; version=0.0.4";

        protected virtual HttpMessageHandler MessageHandler => new HttpClientHandler();

        /// <summary>
        ///     Constructor
        /// </summary>
        public MetricPusher()
        {
            _httpClient = new HttpClient(MessageHandler);
        }

        /// <inheritdoc />
        public async Task PushAsync(IEnumerable<CMetricFamily> metricFamilies, string endpoint, string job, string instance, string contentType)
        {
            await PushAsync(metricFamilies, new[] { endpoint }, job, instance, contentType).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task PushAsync(IEnumerable<CMetricFamily> metrics, string[] endpoints, string job, string instance, string contentType)
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient();
            }
            var cntType = _contentType;
            if (!string.IsNullOrEmpty(contentType))
            {
                cntType = contentType;
            }
            if (string.IsNullOrEmpty(job))
            {
                throw new ArgumentNullException(nameof(job));
            }

            var tasks = new List<Task<HttpResponseMessage>>(endpoints.Length);
            var streamsToDispose = new List<Stream>();

            foreach (var endpoint in endpoints)
            {
                var memoryStream = new MemoryStream();
                streamsToDispose.Add(memoryStream);
                ScrapeHandler.ProcessScrapeRequest(metrics, cntType, memoryStream);
                memoryStream.Position = 0;
                var debugString = Encoding.UTF8.GetString(memoryStream.ToArray());
                System.Diagnostics.Debug.WriteLine(debugString);

                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentNullException(nameof(endpoint));
                }

                var url = $"{endpoint.TrimEnd('/')}/metrics/job/{job}";
                if (!string.IsNullOrEmpty(instance))
                {
                    url = $"{url}/instance/{instance}";
                }

                if (!Uri.TryCreate(url, UriKind.Absolute, out var targetUrl))
                {
                    throw new ArgumentException("Endpoint must be a valid url", nameof(endpoint));
                }

                var streamContent = new StreamContent(memoryStream);
                tasks.Add(_httpClient.PostAsync(targetUrl, streamContent));
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
            Exception exception = null;
            foreach (var task in tasks)
            {
                var response = await task.ConfigureAwait(false);
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            streamsToDispose.ForEach(s => s.Dispose());

            if (exception != null)
            {
                throw exception;
            }
        }
    }
}