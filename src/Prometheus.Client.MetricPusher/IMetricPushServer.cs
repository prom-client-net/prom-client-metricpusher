namespace Prometheus.Client.MetricPusher
{
    public interface IMetricPushServer
    {
        /// <summary>
        ///     Server is Running?
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     Start Metric Push Server
        /// </summary>
        void Start();

        /// <summary>
        ///     Stop Metric Push Server
        /// </summary>
        void Stop();
    }
}
