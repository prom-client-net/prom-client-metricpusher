using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests
{
    // Need Environment
    // https://github.com/prometheus/pushgateway
    
    public class MetricPusherTests
    {        
        [Fact]
        public async Task Simple_Push()
        {
            var counter = Metrics.CreateCounter("test_c", "help");
            counter.Inc();
            
            var pusher = new MetricPusher("http://localhost:9091", "pushgateway-test", "test-instance");
            
            await pusher.PushAsync();
        }
    }
}