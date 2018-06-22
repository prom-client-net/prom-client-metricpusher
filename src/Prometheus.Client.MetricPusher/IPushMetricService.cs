using System.Collections.Generic;
using System.Threading.Tasks;
using Prometheus.Client.Contracts;

namespace Prometheus.Client.MetricPusher
{
    public interface IMetricPushService
    {
        /// <summary>
        /// Push metrics to single pushgateway endpoint
        /// </summary>
        /// <param name="metricFamilies">Collection of metrics</param>
        /// <param name="endpoint">PushGateway endpoint</param>
        /// <param name="job">job name</param>
        /// <param name="instance">instance name</param>
        /// <param name="contentType">content-type</param>
        /// <returns></returns>
        Task PushAsync(IEnumerable<CMetricFamily> metricFamilies, string endpoint, string job, string instance,
            string contentType);
        /// <summary>
        /// Push metrics to single pushgateway endpoint
        /// </summary>
        /// <param name="metrics">Collection of metrics</param>
        /// <param name="endpoints">PushGateway endpoints - fault-tolerance</param>
        /// <param name="job">job name</param>
        /// <param name="instance">instance name</param>
        /// <param name="contentType">content-type</param>
        /// <returns></returns>
        Task PushAsync(IEnumerable<CMetricFamily> metrics, string[] endpoints, string job, string instance,
            string contentType);
    }
}
