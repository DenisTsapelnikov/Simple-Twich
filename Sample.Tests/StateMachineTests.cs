using System;
using System.Linq;
using System.Threading.Tasks;
using Automatonymous.Graphing;
using Automatonymous.Visualizer;
using Components.StateMachines;
using Contracts;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Sample.Tests
{
    public class Submitting_an_order : IAsyncDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly InMemoryTestHarness _harness;
        private readonly OrderStateMachine _stateMachine;
        private readonly StateMachineSagaTestHarness<OrderState, OrderStateMachine> _saga;

        public Submitting_an_order(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            this._harness = new InMemoryTestHarness();
            this._stateMachine = new OrderStateMachine();
            this._saga = _harness.StateMachineSaga<OrderState, OrderStateMachine>(_stateMachine);
            _harness.Start();
        }

        [Fact]
        public async Task Should_create_a_state_instance()
        {
            var orderId = NewId.NextGuid();
            await _harness.Bus.Publish<OrderSubmitted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "customer"
            });

            _harness.Published.Select<OrderSubmitted>().Any().Should().BeTrue();
            _harness.Consumed.Select<OrderSubmitted>().Any().Should().BeTrue();
            var instanceId = await _saga.Exists(orderId, x => x.Submitted);
            instanceId.Should().NotBeNull();

            var instance = _saga.Sagas.Contains(instanceId.Value);
            instance.CustomerNumber.Should().Be("customer");
        }

        [Fact]
        public async Task Should_respond_to_status_checks()
        {
            var orderId = NewId.NextGuid();
            await _harness.Bus.Publish<OrderSubmitted>(new
            {
                OrderId = orderId, InVar.Timestamp, CustomerNumber = "customer"
            });
            _harness.Consumed.Select<OrderSubmitted>().Any().Should().BeTrue();
            var requestClient = await _harness.ConnectRequestClient<CheckOrder>();
            var response = await requestClient.GetResponse<OrderStatus>(new {OrderId = orderId});
            response.Message.State.Should().Be(_stateMachine.Submitted?.Name);
        }

        [Fact]
        public async Task Should_cancel_when_custom_account_closed()
        {
            var orderId = NewId.NextGuid();
            await _harness.Bus.Publish<OrderSubmitted>(new
            {
                OrderId = orderId, InVar.Timestamp, CustomerNumber = "customer"
            });

            var instanceId = await _saga.Exists(orderId, x => x.Submitted);
            instanceId.Should().NotBeNull();

            await _harness.Bus.Publish<CustomAccountClosed>(new
            {
                CustomerId = InVar.Id,
                CustomerNumber = "customer"
            });

            instanceId = await _saga.Exists(orderId, x => x.Canceled);
            instanceId.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_accept_when_order_is_accepted()
        {
            var orderId = NewId.NextGuid();
            await _harness.Bus.Publish<OrderSubmitted>(new
            {
                OrderId = orderId,
                InVar.Timestamp,
                CustomerNumber = "customer"
            });

            _saga.Created.Select(r => r.CorrelationId == orderId).Any().Should().BeTrue();

            var instanceId = await _saga.Exists(orderId, x => x.Submitted);
            instanceId.Should().NotBeNull();

            await _harness.Bus.Publish<OrderAccepted>(new {OrderId = orderId, InVar.Timestamp});
            instanceId = await _saga.Exists(orderId, x => x.Accepted);
            instanceId.Should().NotBeNull();
        }

        [Fact]
        public void Show_me_the_state_machine()
        {
            var orderStateMachine = new OrderStateMachine();
            var graph = orderStateMachine.GetGraph();
            var generator = new StateMachineGraphvizGenerator(graph);
            var dots = generator.CreateDotFile();
            _testOutputHelper.WriteLine(dots);
        }

        public async ValueTask DisposeAsync()
        {
            await _harness.Stop();
        }
    }
}