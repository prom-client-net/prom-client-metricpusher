# Prometheus.Client.MetricPusher

[![NuGet](https://img.shields.io/nuget/v/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![NuGet](https://img.shields.io/nuget/dt/Prometheus.Client.MetricPusher.svg)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![CI](https://img.shields.io/github/workflow/status/prom-client-net/prom-client-metricpusher/%F0%9F%92%BF%20CI%20Master?label=CI&logo=github)](https://github.com/prom-client-net/prom-client-metricpusher/actions/workflows/master.yml)
[![License MIT](https://img.shields.io/badge/license-MIT-green.svg)](https://opensource.org/licenses/MIT) 

## PushGateway

Sometimes when it is not possible to pull e.g. - nodes behind LB or there is a worker like daemon or windows service that does not have HTTP endpoint still there is way to push your metrics to PushGateaway server that you can install from [here](https://github.com/prometheus/pushgateway/releases).

## Installation

```shell
dotnet add package Prometheus.Client.MetricPusher
```

## Quick start

[Examples](https://github.com/prom-client-net/prom-examples)

One push. You can use Timer for regularly push to PushGateway:

```c#
var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = "http://localhost:9091", Job = "pushgateway", Instance = "instance" });
await pusher.PushAsync(); 

```

Push with Auth:

```c#
var pusher = new MetricPusher(new MetricPusherOptions
{
    Endpoint = "http://localhost:9091",
    Job = "pushgateway",
    Instance = "instance",
    AdditionalHeaders = new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } }
});
```

Background server:

```c#
var pusher = new MetricPusher(new MetricPusherOptions { Endpoint = "http://localhost:9091", Job = "pushgateway" });

var worker = new MetricPushServer(pusher); 
worker.Start(); 
...
worker.Stop(); 

```

## Contribute

Contributions to the package are always welcome!

* Report any bugs or issues you find on the [issue tracker](https://github.com/prom-client-net/prom-client-metricpusher/issues).
* You can grab the source code at the package's [git repository](https://github.com/prom-client-net/prom-client-metricpusher).

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).
