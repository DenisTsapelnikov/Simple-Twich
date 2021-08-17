using System;

namespace Contracts
{
    public record FulfillOrder
    {
        public Guid OrderId { get; init; }
    }
}