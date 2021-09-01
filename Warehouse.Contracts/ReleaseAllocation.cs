using System;

namespace Warehouse.Contracts
{
    public interface ReleaseAllocationRequested
    {
        Guid AllocationId { get; set; }  
        string Reason { get; set; }
    }
}