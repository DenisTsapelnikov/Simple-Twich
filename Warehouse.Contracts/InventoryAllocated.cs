using System;

namespace Warehouse.Contracts
{
    public record InventoryAllocated
    {
        public Guid AllocationId { get; init; } 
        public string ItemNumber { get; init; }
        public decimal Quantity { get; init; }
    }
}