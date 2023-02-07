using System;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher
{
    /// <summary>
    ///     Service for Push Gateway
    /// </summary>
    public interface IMetricPusher : IDisposable
    {
        /// <summary>
        ///     Push metrics to single pushgateway endpoint
        /// </summary>
        Task PushAsync();
    }
}
