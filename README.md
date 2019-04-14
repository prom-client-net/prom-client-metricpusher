# Prometheus.Client.MetricPusher

[![MyGet](https://img.shields.io/myget/prometheus-client-net/vpre/Prometheus.Client.MetricPusher.svg?label=myget)](https://www.myget.org/feed/prometheus-client-net/package/nuget/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/v/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/dt/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)

[![Build status](https://ci.appveyor.com/api/projects/status/46twv9kokpnv6952?svg=true)](https://ci.appveyor.com/project/PrometheusClientNet/prometheus-client-metricpusher)
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

If you are having problems, send a mail to [prometheus@phnx47.net](mailto://prometheus@phnx47.net). I will try to help you.

I would also very much appreciate your support by buying me a coffee.

<a href="https://www.buymeacoffee.com/phnx47" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/yellow_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).


