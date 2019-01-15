using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Prometheus.Client.MetricPusher.Tests
{
    // Need Environment
    // https://github.com/prometheus/pushgateway

    public class MetricPusherTests
    {
        private readonly ITestOutputHelper _output;
        
        public MetricPusherTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async Task Simple_Push()
        {
            var counter = Metrics.CreateCounter("test_c12", "help");
            counter.Inc();

            var pusher = new MetricPusher("http://localhost:9091", "pushgateway-test", new Dictionary<string, string>(), "instance");
            await pusher.PushAsync();
        }

        [Fact]
        public async Task Worker_10Step()
        {
            var counter = Metrics.CreateCounter("worker_counter1", "help");
            var pusher = new MetricPusher("http://localhost:9091", "pushgateway-testworker", new Dictionary<string, string>());
            
            var worker = new MetricPushServer(pusher);
            worker.Start();

            for (int i = 0; i < 10; i++)
            {
                counter.Inc();
                _output.WriteLine($"Step: {i}, IsRunning: {worker.IsRunning}");
                
                switch (i)
                {
                    case 5:
                        worker.Stop();
                        break;
                    case 8:
                        worker.Start();
                        break;
                }

                await Task.Delay(2000);
            }
            worker.Stop();
        }

        [Fact]
        public async Task TestPushContinuesOnError()
        {
            var pusher = new TestPusher(async () => throw new Exception("Push error"));
            
            var worker = new MetricPushServer(pusher, TimeSpan.FromSeconds(0.05));
            worker.Start();
            await Task.Delay(150);
            Assert.Equal(3, pusher.PushCounter);
            worker.Stop();
        }
    }       
    
}
