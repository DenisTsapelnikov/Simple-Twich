using System;
using MassTransit.Courier.Contracts;

namespace Contracts
{
    public record CheckOrder
    {
        public Guid OrderId { get; init; }
    }

    public record OrderFulfillmentFaulted
    {
        public Guid OrderId { get; init; }
        public DateTime Time { get; init; }
    }
    
    public record OrderFulfillmentCompleted
    {
        public Guid OrderId { get; init; }
        public DateTime Time { get; init; }
    }
}