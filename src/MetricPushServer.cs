using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher;

public class MetricPushServer(IMetricPusher[] metricPushers, TimeSpan pushInterval) : IMetricPushServer
{
    private CancellationTokenSource _cts;
    private Task _task;

    public MetricPushServer(IMetricPusher metricPusher)
        : this([metricPusher])
    {
    }

    public MetricPushServer(IMetricPusher metricPusher, TimeSpan pushInterval)
        : this([metricPusher], pushInterval)
    {
    }

    public MetricPushServer(IMetricPusher[] metricPushers)
        : this(metricPushers, TimeSpan.FromMilliseconds(1000))
    {
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
        async Task DoPushAsync(IMetricPusher pusher)
        {
            try
            {
                await pusher.PushAsync();
            }
            catch (Exception ex)
            {
                OnPushError(pusher, ex);
            }
        }

        return Task.Run(async () =>
        {
            var innerTasks = metricPushers.Select(metricPusher => Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    await DoPushAsync(metricPusher);

                    try
                    {
                        await Task.Delay(pushInterval, _cts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                }

                // Push the very last metric values before exit
                await DoPushAsync(metricPusher);
            }));

            await Task.WhenAll(innerTasks);
        }, _cts.Token);
    }

    protected virtual void OnPushError(IMetricPusher metricPusher, Exception exception)
    {
    }
}
