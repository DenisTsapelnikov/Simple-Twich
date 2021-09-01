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
            await context.Publish<AllocationCreated>(new {AllocationId = context.Message.AllocationId, HoldDuration = TimeSpan.FromSeconds(8)});

            await context.RespondAsync<InventoryAllocated>(new 
            {
                ItemNumber = context.Message.ItemNumber,
                Quantity = context.Message.Quantity,
                AllocationId = context.Message.AllocationId
            });
        }
    }
}