using System;
using System.Threading.Tasks;

namespace Prometheus.Client.MetricPusher.Tests
{
    public class TestPusher : IMetricPusher
    {
        public int PushCounter { get; private set; }
        private readonly Func<Task> _pushAction;

        public TestPusher(Func<Task> pushAction)
        {
            _pushAction = pushAction;
        }

        public async Task PushAsync()
        {
            PushCounter++;
            await _pushAction();
        }
    }
}
