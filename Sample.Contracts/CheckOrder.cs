using System;

namespace Contracts
{
    public record CheckOrder
    {
        public Guid OrderId { get; init; }
    }

    public record OrderFulfillmentFaulted
    {
        public Guid OrderId { get; set; }
        public DateTime Time { get; set; }
    }
}