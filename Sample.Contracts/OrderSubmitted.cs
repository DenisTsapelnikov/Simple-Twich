using System;
using System.Drawing;

namespace Contracts
{
    public record OrderSubmitted(Guid OrderId, DateTime Timestamp, string? CustomerNumber, string PaymentCardNumber)
    {
        public OrderSubmitted() : this(Guid.Empty, DateTime.MinValue, string.Empty, string.Empty)
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