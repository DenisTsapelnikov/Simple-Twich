using System;

namespace Warehouse.Contracts
{
    public interface AllocationCreated
    {
        Guid AllocationId { get; set; }
        TimeSpan HoldDuration { get; set; }
    }
}