using System;

namespace Contracts
{
    public interface OrderStatus
    {
        Guid CorrelationId { get; set; }
        string? State { get; set; }
        string? CustomerNumber { get; set; }
    }
}