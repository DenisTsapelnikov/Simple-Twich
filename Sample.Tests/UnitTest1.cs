using System;
using System.Linq;
using System.Threading.Tasks;
using Components.Consumers;
using Contracts;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace Sample.Tests
{
    public class When_an_order_request_consumed : IDisposable
    {
        private readonly InMemoryTestHarness _harness;
        private readonly ConsumerTestHarness<SubmitOrderConsumer> _consumer;
        private const string CustomerId = "12345";

        public When_an_order_request_consumed()
        {
            this._harness = new InMemoryTestHarness();
            this._consumer = _harness.Consumer<SubmitOrderConsumer>();
            this._harness.Start();
        }

        [Fact]
        public async Task Should_response_with_accepted_if_ok()
        {
            var orderId = NewId.NextGuid();

            await _harness.InputQueueSendEndpoint.Send(new SubmitOrder(orderId, InVar.Timestamp,
                CustomerId, default));

            _consumer.Consumed.Select<SubmitOrder>().Any().Should().BeTrue();
        }

        [Fact]
        public async Task Should_response_with_order_submission_accepted_if_ok()
        {
            var orderId = NewId.NextGuid();

            var requestClient = await _harness.ConnectRequestClient<SubmitOrder>();
            var response =
                await requestClient.GetResponse<OrderSubmissionAccepted>(new SubmitOrder(orderId, InVar.Timestamp,
                    CustomerId, default));

            _consumer.Consumed.Select<SubmitOrder>().Any().Should().BeTrue();
            _harness.Sent.Select<OrderSubmissionAccepted>().Any().Should().BeTrue();
            response.Message.OrderId.Should().Be(orderId);
        }

        [Fact]
        public async Task Should_response_with_rejected_if_test()
        {
            var orderId = NewId.NextGuid();

            var requestClient = await _harness.ConnectRequestClient<SubmitOrder>();
            var response =
                await requestClient.GetResponse<OrderSubmissionRejected>(
                    new OrderSubmissionRejected() {OrderId = orderId, Timestamp = InVar.Timestamp,CustomerId = "Nylon"});
            response.Message.OrderId.Should().Be(orderId);
            _consumer.Consumed.Select<SubmitOrder>().Any().Should().BeTrue();
            _harness.Sent.Select<OrderSubmissionRejected>().Any().Should().BeTrue();
        }

        [Fact]
        public async Task Should_publish_order_submitted_event()
        {
            var orderId = NewId.NextGuid();

            await _harness.InputQueueSendEndpoint.Send(new SubmitOrder(orderId, InVar.Timestamp,
                "customer", default));

            _harness.Published.Select<OrderSubmitted>().Any().Should().BeTrue();
        }

        [Fact]
        public async Task Should_not_publish_order_submitted_event_when_rejected()
        {
            var orderId = NewId.NextGuid();
            _harness.TestTimeout = TimeSpan.FromSeconds(5);
            await _harness.InputQueueSendEndpoint.Send(new SubmitOrder(orderId, InVar.Timestamp, "Nylon", default));

            _consumer.Consumed.Select<SubmitOrder>().Any().Should().BeTrue();

            _harness.Published.Select<OrderSubmitted>().Any().Should().BeFalse();
        }

        public void Dispose()
        {
            _harness?.Stop();
            _harness?.Dispose();
        }
    }
}