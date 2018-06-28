# Prometheus.Client.MetricPusher

[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

## Installation

	dotnet add package Prometheus.Client.MetricPusher

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).

## PushGateaway
Sometimes when it is not possible to pull e.g. - nodes behind LB or there is a worker like daemon or windows service that does not have HTTP endpoint still there is way to push your metrics to Pushgateway server that you can install from [here](https://github.com/prometheus/pushgateway/releases "here").
### Pushgateway example

```csharp
// collecting metrics wherever you need
var metrics = new CollectorRegistry();
var metricFactory = new MetricFactory(_metrics);
var counter = metricFactory.CreateCounter("test_counter", "just a simple test counter", "Color", "Size");
counter.Labels("White", "XXS").Inc();
counter.Labels("Black", "XXL").Inc();

// when you want to push it. It can be background job / worker that will push collected metrics
// using Timer, while(true) -> Task.Delay(Interval), IHostedService, etc...
var pushService = new MetricPushService()
await pushService.PushAsync(metrics.CollectAll(), "http://localhost:9091", "pushgateway", Environment.MachineName, null);

// Background push worker example:
public class MetricsPushWorker : IDispose
    {
        private Timer _timer;
        private readonly IMetricPushService _pushService;

        public MetricsPushWorker(IMetricPushService pushService)
        {
            _pushService = pushService;
            _timer = new Timer(async (e) => await PushMetricsAsync(e), null,  TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        private async Task PushMetricsAsync(object state)
        {
            var metricFamilies = CollectorRegistry.Instance.CollectAll();
            await _pushService.PushAsync(metricFamilies, "htpp://localhost:9091", "pushgateway", Environment.MachineName, null).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
```


