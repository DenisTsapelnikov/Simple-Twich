using System;
using Automatonymous;
using Automatonymous.CorrelationConfigurators;
using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.StateMachines
{
    public class AllocationStateMachine : MassTransitStateMachine<AllocationState>
    {
        public AllocationStateMachine()
        {
            Event(() => AllocationCreated, x => x.CorrelateById(m => m.Message.AllocationId));

            Schedule(() => HoldExpiration, x => x.HoldDurationToken,
                configurator =>
                {
                    Console.WriteLine("Message scheduled");
                    configurator.Delay = TimeSpan.FromHours(1);
                    configurator.Received = s => s.CorrelateById(m => m.Message.AllocationId);
                });

            InstanceState(state => state.CurrentState);
            Initially(
                When(AllocationCreated)
                    .Schedule(HoldExpiration,
                        context => context.Init<AllocationHoldDurationExpired>(new {context.Data.AllocationId}),
                        c => c.Data.HoldDuration)
                    .TransitionTo(Allocated));
            
            During(Allocated, 
                When(HoldExpiration.Received).Then(context =>
                    {
                        Console.WriteLine("Hold expiration received: {0} event name: {1}", context.Data.AllocationId, context.Event.Name);
                    })
                    .Finalize());
            
            SetCompletedWhenFinalized();
        }

        public Schedule<AllocationState, AllocationHoldDurationExpired> HoldExpiration { get; set; }
        public State Allocated { get; set; }
        public Event<AllocationCreated> AllocationCreated { get; set; }
    }
}