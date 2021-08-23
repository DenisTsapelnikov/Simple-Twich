using GreenPipes;
using MassTransit;
using MassTransit.Definition;

namespace Warehouse.Components.StateMachines
{
    public class AllocationStateMachineDefinition : SagaDefinition<AllocationState>
    {
        public AllocationStateMachineDefinition()
        {
            ConcurrentMessageLimit = 4;
        }

        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<AllocationState> sagaConfigurator)
        {
            endpointConfigurator.UseMessageRetry(r => r.Interval(3, 1000));
            endpointConfigurator.UseInMemoryOutbox();
            // base.ConfigureSaga(endpointConfigurator, sagaConfigurator);
        }
    }
}