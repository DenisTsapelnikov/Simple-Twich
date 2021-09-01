using System;
using System.Drawing;

namespace Contracts
{
    public interface OrderSubmitted
    {
        public Guid OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string? CustomerNumber { get; set; }
        public string PaymentCardNumber { get; set; }
   }

    public interface CustomAccountClosed
    {
        public Guid CustomerId { get; set; }
        public string CustomerNumber { get; set; }

    }
}