using System;
using MassTransit.Initializers.Variables;

namespace Contracts
{
    public record OrderSubmissionRejected(Guid OrderId, TimestampVariable Timestamp, string CustomerId,
        string Reason)
    {
        public OrderSubmissionRejected() : this(default, default, default, default)
        {
        }
    }
}