using System;
using System.Threading.Tasks;
using Contracts;
using MassTransit;

namespace Components.Consumers
{
    public class FaultConsumer : IConsumer<Fault<FulfillOrder>>
    {
        public Task Consume(ConsumeContext<Fault<FulfillOrder>> context)
        {
            Console.WriteLine("Hello from {0} ", nameof(FaultConsumer));
            return Task.CompletedTask;
        }
    }
}