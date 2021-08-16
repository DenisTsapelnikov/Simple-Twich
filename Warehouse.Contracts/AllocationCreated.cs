using System;

namespace Warehouse.Contracts
{
    public record AllocationCreated
    {
        public Guid AllocationId { get; init; }
        public TimeSpan HoldDuration { get; init; }
    }
}