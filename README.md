# Prometheus.Client.MetricPusher

[![NuGet](https://img.shields.io/nuget/v/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/dt/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![CI](https://github.com/PrometheusClientNet/Prometheus.Client.MetricPusher/workflows/CI/badge.svg)](https://github.com/PrometheusClientNet/Prometheus.Client.MetricPusher/actions?query=workflow%3ACI)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

## PushGateaway

Sometimes when it is not possible to pull e.g. - nodes behind LB or there is a worker like daemon or windows service that does not have HTTP endpoint still there is way to push your metrics to Pushgateway server that you can install from [here](https://github.com/prometheus/pushgateway/releases "here").

## Installation

	dotnet add package Prometheus.Client.MetricPusher

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).


## Quik start

[Examples](https://github.com/PrometheusClientNet/Prometheus.Client.Examples/tree/master/MetricPusher)

One push. You can use Timer for regularly push to PushGateway:
```csharp
var pusher = new MetricPusher("http://localhost:9091", "pushgateway", "instance");
await pusher.PushAsync();
```

Push with Auth:
```csharp
var pusher = new MetricPusher("http://localhost:9091", "pushgateway", "instance", new Dictionary<string, string>
{
    { "Authorization", "Bearer " + accessToken }
});
```

Backgroud server:

```csharp
var pusher = new MetricPusher("http://localhost:9091", "pushgateway");
            
var worker = new MetricPushServer(pusher);
worker.Start();
...
worker.Stop();

```

## Contribute

Contributions to the package are always welcome!

* Report any bugs or issues you find on the [issue tracker](https://github.com/PrometheusClientNet/Prometheus.Client.MetricPusher/issues).
* You can grab the source code at the package's [git repository](https://github.com/PrometheusClientNet/Prometheus.Client.MetricPusher).

## Support

I would also very much appreciate your support:

<a href="https://www.buymeacoffee.com/phnx47"><img width="32px" src="https://raw.githubusercontent.com/phnx47/files/master/button-sponsors/bmac0.png" alt="Buy Me A Coffee"></a>
<a href="https://ko-fi.com/phnx47"><img width="32px" src="https://raw.githubusercontent.com/phnx47/files/master/button-sponsors/kofi0.png" alt="Support me on ko-fi"></a>
<a href="https://www.patreon.com/phnx47"><img width="32px" src="https://raw.githubusercontent.com/phnx47/files/master/button-sponsors/patreon0.png" alt="Support me on Patreon"></a>

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).


