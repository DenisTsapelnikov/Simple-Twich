using System;
using System.Net;
using System.Threading.Tasks;
using MassTransit.Courier;

namespace Components.CourierActivities
{
    public class PaymentActivity : IActivity<PaymentArguments, PaymentLog>
    {
        public async Task<ExecutionResult> Execute(ExecuteContext<PaymentArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber;
            if (string.IsNullOrEmpty(cardNumber))
            {
                throw new ArgumentNullException(nameof(cardNumber));
            }

            if (cardNumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("The card number was invalid");
            }

            await Task.Delay(100);
            return context.Completed(new {AuthorizationCode = "77777"});
        }

        public async Task<CompensationResult> Compensate(CompensateContext<PaymentLog> context)
        {
            await Task.Delay(100);
            return context.Compensated();
        }
    }

    public record PaymentArguments
    {
        public Guid OrderId { get; init; }
        public decimal Amount { get; init; }
        public string CardNumber { get; init; }
    }

    public record PaymentLog
    {
        public string AuthorizationCode { get; init; }
    }
}