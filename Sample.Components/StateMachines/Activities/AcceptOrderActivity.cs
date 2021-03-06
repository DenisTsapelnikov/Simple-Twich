using System;
using System.Threading.Tasks;
using Automatonymous;
using Contracts;
using GreenPipes;
using MassTransit;

namespace Components.StateMachines.Activities
{
    public class AcceptOrderActivity : Activity<OrderState, OrderAccepted>
    {
        public void Probe(ProbeContext context)
        {
            context.CreateScope("accept-order");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<OrderState, OrderAccepted> context,
            Behavior<OrderState, OrderAccepted> next)
        {
            Console.WriteLine("Hello, world. Order is {0}", context.Data.OrderId);
            var consumeContext = context.GetPayload<ConsumeContext>();

            var sendEndpoint = await consumeContext.GetSendEndpoint(new Uri("exchange:fulfill-order"));
            await sendEndpoint.Send(new FulfillOrder(context.Data.OrderId));
            
            await next.Execute(context).ConfigureAwait(false);
        }

        public Task Faulted<TException>(BehaviorExceptionContext<OrderState, OrderAccepted, TException> context,
            Behavior<OrderState, OrderAccepted> next) where TException : Exception
        {
            return next.Faulted(context);
        }
    }
}