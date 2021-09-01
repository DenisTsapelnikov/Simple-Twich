using System;
using MassTransit.Initializers.Variables;

namespace Contracts
{
    public interface OrderAccepted
    {
        Guid OrderId { get; }
        TimestampVariable Timestamp { get; }
    }
}