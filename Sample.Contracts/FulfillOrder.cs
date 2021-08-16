using System;

namespace Contracts
{
    public record FulfillOrder(Guid OrderId)
    {
        public FulfillOrder() : this(Guid.Empty)
        {
        }
    }
}