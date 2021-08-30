using System;

namespace Contracts
{
    public record SubmitOrder
    {
        public SubmitOrder()
        {
        }
        public SubmitOrder(Guid orderId, DateTime timestamp, string customerId, string paymentCardNumber)
        {
            OrderId = orderId;
            Timestamp = timestamp;
            CustomerId = customerId;
            PaymentCardNumber = paymentCardNumber;
        }

        public Guid OrderId { get; init; }
        public DateTime Timestamp { get; init; }
        public string CustomerId { get; init; }
        public string PaymentCardNumber { get; set; }
    }
}