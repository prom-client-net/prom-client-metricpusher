using System;
using System.Linq;
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

        public bool IsRunning => _task != null;

        public void Start()
        {
            if (IsRunning)
                return;

            _cts = new CancellationTokenSource();
            _task = Run();
        }

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
                var innerTasks = _metricPushers.Select(metricPusher => Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            await metricPusher.PushAsync();
                        }
                        catch (Exception ex)
                        {
                            OnPushError(metricPusher, ex);
                        }

                        await Task.Delay(_pushInterval, _cts.Token);
                    }
                }, _cts.Token));

                await Task.WhenAll(innerTasks);
            }, _cts.Token);
        }

        protected virtual void OnPushError(IMetricPusher metricPusher, Exception exception)
        {
        }
    }
}
