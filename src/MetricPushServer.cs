using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher;

/// <summary>
/// Server that periodically pushes metrics using configured pushers.
/// </summary>
public class MetricPushServer(IMetricPusher[] metricPushers, TimeSpan pushInterval) : IMetricPushServer
{
    private CancellationTokenSource _cts;
    private Task _task;

    /// <summary>
    /// Initialize a new instance of the <see cref="MetricPushServer"/> class with a single pusher.
    /// </summary>
    /// <param name="metricPusher">The <see cref="IMetricPusher"/> instance.</param>
    public MetricPushServer(IMetricPusher metricPusher)
        : this([metricPusher])
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="MetricPushServer"/> class with a single pusher and interval.
    /// </summary>
    /// <param name="metricPusher">The <see cref="IMetricPusher"/> instance.</param>
    /// <param name="pushInterval">The interval between pushes.</param>
    public MetricPushServer(IMetricPusher metricPusher, TimeSpan pushInterval)
        : this([metricPusher], pushInterval)
    {
    }

    /// <summary>
    /// Initialize a new instance of the <see cref="MetricPushServer"/> class with multiple pushers.
    /// </summary>
    /// <param name="metricPushers">The array of <see cref="IMetricPusher"/> instances.</param>
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
    }

    protected virtual void OnPushError(IMetricPusher metricPusher, Exception exception)
    {
    }
}
