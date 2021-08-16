using System;

namespace Contracts
{
    public record OrderNotFound(Guid MessageOrderId)
    {
        public OrderNotFound() : this(Guid.Empty)
        {
        }
    }
}