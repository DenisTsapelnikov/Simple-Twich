using System;

namespace Warehouse.Contracts
{
    public record ReleaseAllocationRequested
    {
        public Guid AllocationId { get; init; }  
        public string Reason { get; init; }
    }
}