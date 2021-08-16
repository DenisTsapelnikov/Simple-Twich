using System;

namespace Warehouse.Contracts
{
    public record AllocationHoldDurationExpired
    {
        public Guid AllocationId { get; init; }
    }
}