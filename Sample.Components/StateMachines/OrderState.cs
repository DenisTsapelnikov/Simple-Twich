using System;
using Automatonymous;
using MassTransit.Saga;
using MongoDB.Bson.Serialization.Attributes;

namespace Components.StateMachines
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion 
    {           
        [BsonId]
        public Guid CorrelationId { get; set; }
        public string? CurrentState { get; set; }
        public int Version { get; set; }
        public DateTime Updated { get; set; }
        public string? CustomerNumber { get; set; }
        public DateTime? SubmitDate { get; set; }
    }
}