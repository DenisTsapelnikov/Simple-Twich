using System;

namespace Contracts
{
    public record OrderSubmissionAccepted(Guid OrderId, DateTime Timestamp, string CustomerId)
    {
        public OrderSubmissionAccepted() : this(default, default, default)
        {
        }
    }
}