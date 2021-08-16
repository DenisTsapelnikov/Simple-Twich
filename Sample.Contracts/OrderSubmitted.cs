using System;
using System.Drawing;

namespace Contracts
{
    public record OrderSubmitted(Guid OrderId, DateTime Timestamp, string? CustomerNumber)
    {
        public OrderSubmitted() : this(Guid.Empty, DateTime.MinValue, string.Empty)
        {
        }
    };

    public record CustomAccountClosed(Guid CustomerId, string CustomerNumber)
    {
        public CustomAccountClosed() : this(Guid.Empty, string.Empty)
        {
        }
    };
}