using System;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher
{
    public class MetricPushServer : IMetricPushServer
    {
        private readonly IMetricPusher[] _metricPushers;
        private readonly TimeSpan _pushInterval;
        private CancellationTokenSource _cts;
        private Task _task;
        
        public MetricPushServer(IMetricPusher metricPusher)
            : this(new[] { metricPusher })
        {
        }
        
        public MetricPushServer(IMetricPusher metricPusher, TimeSpan pushInterval)
            : this(new[] { metricPusher }, pushInterval)
        {
        }

        public MetricPushServer(IMetricPusher[] metricPushers)
            : this(metricPushers, TimeSpan.FromMilliseconds(1000))
        {
        }

        public MetricPushServer(IMetricPusher[] metricPushers, TimeSpan pushInterval)
        {
            _metricPushers = metricPushers;
            _pushInterval = pushInterval;
        }

        /// <inheritdoc />
        public bool IsRunning => _task != null;

        /// <inheritdoc />
        public void Start()
        {
            if (IsRunning)
                return;

            _cts = new CancellationTokenSource();
            _task = Run();
        }

        /// <inheritdoc />
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