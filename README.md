# Prometheus.Client.MetricPusher

[![ci](https://img.shields.io/github/actions/workflow/status/prom-client-net/prom-client-metricpusher/ci.yml?branch=main&label=ci&logo=github&style=flat-square)](https://github.com/prom-client-net/prom-client-metricpusher/actions/workflows/ci.yml)
[![nuget](https://img.shields.io/nuget/v/Prometheus.Client.MetricPusher?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![nuget](https://img.shields.io/nuget/dt/Prometheus.Client.MetricPusher?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Prometheus.Client.MetricPusher)
[![codefactor](https://img.shields.io/codefactor/grade/github/prom-client-net/prom-client-metricpusher?logo=codefactor&style=flat-square)](https://www.codefactor.io/repository/github/prom-client-net/prom-client-metricpusher)
[![license](https://img.shields.io/github/license/prom-client-net/prom-client-metricpusher?style=flat-square)](https://github.com/prom-client-net/prom-client-metricpusher/blob/main/LICENSE)

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
var pusher = new MetricPusher(new MetricPusherOptions
{
    Endpoint = "http://localhost:9091",
    Job = "pushgateway",
    Instance = "instance"
});

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
var pusher = new MetricPusher(new MetricPusherOptions
{
    Endpoint = "http://localhost:9091",
    Job = "pushgateway"
});

var worker = new MetricPushServer(pusher);
worker.Start();
// code
worker.Stop();
```

## Contribute

Contributions to the package are always welcome!

* Report any bugs or issues you find on the [issue tracker](https://github.com/prom-client-net/prom-client-metricpusher/issues).
* You can grab the source code at the package's [git repository](https://github.com/prom-client-net/prom-client-metricpusher).

## License

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).
