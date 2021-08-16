using System;
using MassTransit.Initializers.Variables;

namespace Contracts
{
    public record OrderAccepted(Guid OrderId, TimestampVariable Timestamp)
    {
        public OrderAccepted() : this(default, default)
        {
        }
    }
}