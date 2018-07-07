# Prometheus.Client.MetricPusher

[![MyGet](https://img.shields.io/myget/phnx47-beta/vpre/Prometheus.Client.MetricPusher.svg)](https://www.myget.org/feed/phnx47-beta/package/nuget/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/v/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/dt/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)

[![Build status](https://ci.appveyor.com/api/projects/status/46twv9kokpnv6952?svg=true)](https://ci.appveyor.com/project/PrometheusClientNet/prometheus-client-metricpusher)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

## Installation

	dotnet add package Prometheus.Client.MetricPusher

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).

## PushGateaway

Sometimes when it is not possible to pull e.g. - nodes behind LB or there is a worker like daemon or windows service that does not have HTTP endpoint still there is way to push your metrics to Pushgateway server that you can install from [here](https://github.com/prometheus/pushgateway/releases "here").

### Pushgateway example

One push. You can use Timer for regularly push to PushGateway:
```csharp
var pusher = new MetricPusher("http://localhost:9091", "pushgateway", "instance");
await pusher.PushAsync();

```

Backgroud server:

```csharp
var pusher = new MetricPusher("http://localhost:9091", "pushgateway");
            
var worker = new MetricPushServer(pusher);
worker.Start();
...
worker.Stop();

```


