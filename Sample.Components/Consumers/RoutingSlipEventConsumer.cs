using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace Components.Consumers
{
    public class RoutingSlipEventConsumer : 
        IConsumer<RoutingSlipCompleted>,
        IConsumer<RoutingSlipFaulted>,
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipActivityFaulted>
    {
        private readonly ILogger<RoutingSlipEventConsumer> _logger;

        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Routing Slip completed: {0}", context.Message.TrackingNumber);
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Routing Slip Activity '{ActivityName}' completed: {TrackingNumber}", context.Message.TrackingNumber,
                    context.Message.ActivityName);
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Routing Slip faulted: {TrackingNumber} {ExceptionInfo}", context.Message.TrackingNumber, context.Message.ActivityExceptions.FirstOrDefault());
            }

            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipActivityFaulted> context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Routing Slip Activity '{1}' faulted: {0}", context.Message.TrackingNumber,
                    context.Message.ActivityName);
            }

            return Task.CompletedTask;
        }
    }
}