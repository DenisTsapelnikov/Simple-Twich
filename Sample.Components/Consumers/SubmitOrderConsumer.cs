using System.Threading.Tasks;
using Contracts;
using GreenPipes;
using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using Microsoft.Extensions.Logging;

namespace Components.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer>? _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public SubmitOrderConsumer()
        {
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger?.LogDebug($"Received from: {context.SourceAddress}, message: {context.Message}");
            if (context.Message.CustomerId.Contains("Nylon"))
            {
                if (context.ResponseAddress is not null)
                {
                    await context.RespondAsync(new OrderSubmissionRejected(context.Message.OrderId, InVar.Timestamp,
                        context.Message.CustomerId, "It's always rejected for Nylon"));
                }
                return;
            }

            await context.Publish<OrderSubmitted>(new OrderSubmitted(context.Message.OrderId,InVar.Timestamp,context.Message.CustomerId, context.Message.PaymentCardNumber));

            if (context.ResponseAddress is not null)
            {
                await context.RespondAsync<OrderSubmissionAccepted>(new OrderSubmissionAccepted(context.Message.OrderId,
                    InVar.Timestamp, context.Message.CustomerId));
            }
        }
    }

    public class SubmitOrderConsumerDefinition : ConsumerDefinition<SubmitOrderConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<SubmitOrderConsumer> consumerConfigurator)
        {
            endpointConfigurator.UseRetry(
                configurator => configurator.SetRetryPolicy(p => p.Intervals(500, 5000, 1000)));
            consumerConfigurator.UseInMemoryOutbox();
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }
    }
}