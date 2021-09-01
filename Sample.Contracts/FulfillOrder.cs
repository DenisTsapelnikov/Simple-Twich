using System;

namespace Contracts
{
    public interface FulfillOrder
    {
        Guid OrderId { get; set; }
        string? CustomerNumber { get; set; }
        string? PaymentCardNumber { get; set; }
    }
}