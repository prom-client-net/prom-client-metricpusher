using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher
{
    public class MetricPusherServer
    {
        private readonly IMetricPusher[] _metricPushers;
        private readonly TimeSpan _pushInterval;
        private CancellationTokenSource _cts;
        private Task _task;
        
        public MetricPusherServer(IMetricPusher metricPusher)
            : this(new[] { metricPusher })
        {
        }
        
        public MetricPusherServer(IMetricPusher metricPusher, TimeSpan pushInterval)
            : this(new[] { metricPusher }, pushInterval)
        {
        }

        public MetricPusherServer(IMetricPusher[] metricPushers)
            : this(metricPushers, TimeSpan.FromMilliseconds(1000))
        {
        }

        public MetricPusherServer(IMetricPusher[] metricPushers, TimeSpan pushInterval)
        {
            _metricPushers = metricPushers;
            _pushInterval = pushInterval;
        }

        /// <summary>
        ///     Server is Running?
        /// </summary>
        public bool IsRunning => _task != null;

        /// <summary>
        ///     Start MetricPusher worker
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                return;

            _cts = new CancellationTokenSource();
            _task = Run();
        }

        /// <summary>
        ///     Stop MetricPusher server
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            _cts?.Cancel();
            _task = null;
        }

        private Task Run()
        {
            return Task.Run(async () =>
            {
                foreach (var metricPusher in _metricPushers)
                    while (true)
                    {
                        await metricPusher.PushAsync();
                        await Task.Delay(_pushInterval);
                    }
            }, _cts.Token);
        }
    }
}