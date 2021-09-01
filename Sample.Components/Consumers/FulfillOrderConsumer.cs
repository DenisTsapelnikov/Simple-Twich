using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;
using MassTransit.Courier;
using MassTransit.Courier.Contracts;
using Warehouse.Contracts;

namespace Components.Consumers
{
    public class FulfillOrderConsumer : IConsumer<FulfillOrder>
    {
        public async Task Consume(ConsumeContext<FulfillOrder> context)
        {
            if (context.Message.CustomerNumber!.StartsWith("Invalid"))
            {
                throw new InvalidOperationException("We tried, but the customer is invalid");
            }

            Console.WriteLine("Building RoutingSlip: {0}", nameof(FulfillOrder));
            var builder = new RoutingSlipBuilder(NewId.NextGuid());
            builder.AddActivity("AllocateInventory", new Uri("queue:allocate-inventory_execute"),
                new {ItemNumber = "ITEM23", Quantity = 10.0m});
            builder.AddVariable("OrderId", context.Message.OrderId);

            builder.AddActivity("PaymentActivity", new Uri("queue:payment_execute"),
                new {CardNumber = context.Message.PaymentCardNumber ?? "5999 4000", Amount = 99.95});

            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Faulted | RoutingSlipEvents.Supplemental, RoutingSlipEventContents.None,
                endpoint => endpoint.Send<OrderFulfillmentFaulted>(new {OrderId = context.Message.OrderId}));
            await builder.AddSubscription(context.SourceAddress,
                RoutingSlipEvents.Completed | RoutingSlipEvents.Supplemental, RoutingSlipEventContents.None,
                endpoint => endpoint.Send<OrderFulfillmentCompleted>(new {OrderId = context.Message.OrderId}));

            var routingSlip = builder.Build();
            await context.Execute(routingSlip);
        }
    }
}