using System;
using System.Threading.Tasks;
using MassTransit;
using Warehouse.Contracts;

namespace Warehouse.Components.Consumers
{
    public class AllocateInventoryConsumer : IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
            await context.Publish(new AllocationCreated() {AllocationId = context.Message.AllocationId, HoldDuration = TimeSpan.FromSeconds(8)});

            await context.RespondAsync(new InventoryAllocated
            {
                ItemNumber = context.Message.ItemNumber,
                Quantity = context.Message.Quantity,
                AllocationId = context.Message.AllocationId
            });
        }
    }
}