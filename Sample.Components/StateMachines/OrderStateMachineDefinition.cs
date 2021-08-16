using MassTransit;
using MassTransit.Definition;

namespace Components.StateMachines
{
    public class OrderStateMachineDefinition : SagaDefinition<OrderState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
            ISagaConfigurator<OrderState> sagaConfigurator)
        {
            sagaConfigurator.UseMessageRetry(r => r.SetRetryPolicy(filter => filter.Intervals(500, 5000, 10000)));
            sagaConfigurator.UseInMemoryOutbox();
        }
    }
}