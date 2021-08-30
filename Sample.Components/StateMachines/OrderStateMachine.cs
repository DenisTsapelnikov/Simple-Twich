﻿using System;
using Automatonymous;
using Components.StateMachines.Activities;
using Contracts;
using MassTransit;

namespace Components.StateMachines
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public OrderStateMachine()
        {
            Event(() => OrderSubmitted, e => e.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderAccepted, e => e.CorrelateById(m => m.Message.OrderId));
            Event(() => FulfillmentFaulted, e => e.CorrelateById(m => m.Message.OrderId));
            Event(() => FulfillmentCompleted, e => e.CorrelateById(m => m.Message.OrderId));
            Event(() => OrderStatusRequested, e =>
            {
                e.CorrelateById(m => m.Message.OrderId);
                e.OnMissingInstance(e => e.ExecuteAsync(async context =>
                {
                    if (context.RequestId.HasValue)
                    {
                        await context.RespondAsync<OrderNotFound>(new OrderNotFound(context.Message.OrderId));
                    }
                }));
            });
            Event(() => AccountClosed,
                e => e.CorrelateBy((state, context) => context.Message.CustomerNumber == state.CustomerNumber));

            InstanceState(state => state.CurrentState);

            Initially(
                When(OrderSubmitted).Then(context =>
                {
                    context.Instance.SubmitDate = context.Data.Timestamp;
                    context.Instance.CustomerNumber = context.Data.CustomerNumber;
                    context.Instance.Updated = DateTime.UtcNow;
                    context.Instance.PaymentCardNumber = context.Data.PaymentCardNumber;
                }).TransitionTo(Submitted));

            During(Submitted,
                Ignore(OrderSubmitted),
                When(AccountClosed)
                    .TransitionTo(Canceled),
                When(OrderAccepted)
                    .Activity(selector => selector.OfType<AcceptOrderActivity>())
                    .TransitionTo(Accepted));
            
            During(Accepted, 
                When(FulfillmentFaulted)
                    .TransitionTo(Faulted),
                When(FulfillmentCompleted)
                    .TransitionTo(Completed)
            );
            
            DuringAny(When(OrderStatusRequested).RespondAsync(r => r.Init<OrderStatus>(
                new OrderStatus(r.Instance.CorrelationId, r.Instance.CurrentState, r.Instance.CustomerNumber))));

            DuringAny(When(OrderSubmitted).Then(context =>
            {
                context.Instance.SubmitDate ??= context.Data.Timestamp;
                context.Instance.CustomerNumber ??= context.Data.CustomerNumber;
            }));
        }

        public State? Submitted { get; private set; }
        public State? Accepted { get; private set; }
        public State Canceled { get; private set; }
        public State Faulted { get; private set; }
        public State Completed { get; private set; }

        public Event<OrderSubmitted>? OrderSubmitted { get; private set; }
        public Event<OrderAccepted>? OrderAccepted { get; private set; }
        public Event<CheckOrder>? OrderStatusRequested { get; private set; }

        public Event<CustomAccountClosed> AccountClosed { get; private set; }
        public Event<OrderFulfillmentFaulted> FulfillmentFaulted { get; private set; }
        public Event<OrderFulfillmentCompleted> FulfillmentCompleted { get; private set; }
    }
}