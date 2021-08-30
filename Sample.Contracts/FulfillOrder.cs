using System;

namespace Contracts
{
    public record FulfillOrder
    {
        public Guid OrderId { get; init; }
        public string? CustomerNumber { get; init; }
        public string? PaymentCardNumber { get; init; }
    }
}