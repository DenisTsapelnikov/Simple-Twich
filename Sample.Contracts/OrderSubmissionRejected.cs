using System;
using MassTransit.Initializers.Variables;

namespace Contracts
{
    public interface OrderSubmissionRejected
    {
        Guid OrderId { get; set; }
        TimestampVariable Timestamp { get; set; }
        string CustomerId { get; set; }
        string Reason { get; set; }
    }
}