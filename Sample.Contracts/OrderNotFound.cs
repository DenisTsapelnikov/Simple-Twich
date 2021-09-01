using System;

namespace Contracts
{
    public interface OrderNotFound
    {
        Guid MessageOrderId { get; set; }
    }
}