using System;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace Components.Consumers
{
    public class FulfillOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer>
    {
        public FulfillOrderConsumerDefinition()
        {
            ConcurrentMessageLimit = 4;
        }
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(
                configurator =>
                {
                    configurator.Ignore<InvalidOperationException>();
                    configurator.SetRetryPolicy(p => Retry.Interval(p, 3, new TimeSpan(1000)));
                });
            endpointConfigurator.DiscardFaultedMessages();
        }
    }
}