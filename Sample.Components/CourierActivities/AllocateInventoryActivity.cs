using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Courier;
using Warehouse.Contracts;

namespace Components.CourierActivities
{
    public class AllocateInventoryActivity : IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {
        private readonly IRequestClient<AllocateInventory> _requestClient;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> requestClient)
        {
            _requestClient = requestClient;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var itemNumber = context.Arguments.ItemNumber;
            if (string.IsNullOrEmpty(itemNumber))
            {
                throw new ArgumentNullException(nameof(itemNumber));
            }

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0)
            {
                throw new ArgumentNullException(nameof(quantity));
            }

            var orderId = context.Arguments.OrderId;
            if (orderId == default)
            {
                throw new ArgumentNullException(nameof(orderId));
            }

            var response = await _requestClient.GetResponse<InventoryAllocated>(new AllocateInventory()
                {Quantity = quantity, AllocationId = NewId.NextGuid(), ItemNumber = itemNumber});

            return context.Completed(new {AllocationId = response.Message.AllocationId});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish(new ReleaseAllocationRequested()
                {AllocationId = context.Log.AllocationId, Reason = "Order Faulted"});
            return context.Compensated();
        }
    }

    public record AllocateInventoryArguments
    {
        public Guid OrderId { get; init; }
        public string ItemNumber { get; init; }
        public decimal Quantity { get; init; }
    }

    public record AllocateInventoryLog
    {
        public Guid AllocationId { get; init; }
    }
}