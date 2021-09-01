using System;

namespace Contracts
{
    public interface SubmitOrder
    {
        Guid OrderId { get; init; }
        DateTime Timestamp { get; init; }
        string CustomerId { get; init; }
        string PaymentCardNumber { get; set; }
    }
}