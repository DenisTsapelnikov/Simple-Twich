using System;

namespace Warehouse.Contracts
{
    public interface AllocateInventory
    {
        Guid AllocationId { get; set; }
        string ItemNumber { get; set; }
        decimal Quantity { get; set; }
    }
}