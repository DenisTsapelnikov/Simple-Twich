using System;

namespace Contracts
{
    public interface OrderSubmissionAccepted
    {
        Guid OrderId { get; set; }
        DateTime Timestamp { get; set; }
        string CustomerId { get; set; }
    }
}