using System;

namespace Contracts
{
    public record OrderStatus(Guid CorrelationId, string? State, string? CustomerNumber)
    {
        public OrderStatus():this(default, default,default)
        {
        }
    }
}