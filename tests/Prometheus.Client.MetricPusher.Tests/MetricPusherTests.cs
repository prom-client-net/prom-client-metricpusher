using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prometheus.Client.Collectors;
using Xunit;

namespace Prometheus.Client.MetricPusher.Tests
{
    public class MetricPushServiceTest : BaseMetricPushServiceTest
    {
        private MockedMetricPusher _pusher;
        private ICollectorRegistry _metrics;
        private string _result;
        public override void Act()
        {
            _pusher.PushAsync(_metrics.CollectAll(), "http://localhost:9091",
                "pushgateway", Environment.MachineName, null).GetAwaiter().GetResult();
        }

        public override void Arrange()
        {
            _metrics = new CollectorRegistry();
            var metricFactory = new MetricFactory(_metrics);
            var counter = metricFactory.CreateCounter("test_counter", "just a simple test counter", "Color", "Size");
            counter.Labels("White", "XXS").Inc();
            counter.Labels("Black", "XXL").Inc();

            _pusher = new MockedMetricPusher();
            _pusher.Handler.SetMessageHandler((r) =>
            {
                var streamContent = r.Content as StreamContent;
                _result = streamContent.ReadAsStringAsync().GetAwaiter().GetResult();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });
        }

        [Fact]
        public void Should_Send_Metrics_And_Get_Success()
        {
            var memoryStream = new MemoryStream();
            ScrapeHandler.ProcessScrapeRequest(_metrics.CollectAll(), null, memoryStream);
            var expectedResult = Encoding.UTF8.GetString(memoryStream.ToArray());
            Assert.Equal(expectedResult, _result);
        }
    }

    public abstract class BaseMetricPushServiceTest
    {
        protected BaseMetricPushServiceTest()
        {
            Initialize();
        }

        public void Initialize()
        {
            Arrange();
            Act();
        }

        public abstract void Act();
        public abstract void Arrange();
    }

    public class MockedMetricPusher : MetricPusher
    {
        protected override HttpMessageHandler MessageHandler => Handler;
        public MessageHandler Handler { get; } = new MessageHandler();
    }

    public class MessageHandler : HttpMessageHandler
    {
        private Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;
        public void SetMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await _handler(request).ConfigureAwait(false);
        }
    }
}
