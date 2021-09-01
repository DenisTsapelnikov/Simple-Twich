using System;
using MassTransit.Courier.Contracts;

namespace Contracts
{
    public interface CheckOrder
    {
        public Guid OrderId { get; set; }
    }

    public interface OrderFulfillmentFaulted
    {
        public Guid OrderId { get; set; }
        public DateTime Time { get; set; }
    }
    
    public interface OrderFulfillmentCompleted
    {
        public Guid OrderId { get; set; }
        public DateTime Time { get; set; }
    }
}