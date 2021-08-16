using System;
using System.Threading.Tasks;
using Components.Consumers;
using Contracts;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Mvc;

namespace Simple_Twich.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IRequestClient<SubmitOrder> _requestClient;
        private readonly ISendEndpointProvider _provider;
        private readonly IRequestClient<CheckOrder> _checkOrderClient;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderController(IRequestClient<SubmitOrder> requestClient,
            ISendEndpointProvider provider, IRequestClient<CheckOrder> checkOrderClient,
            IPublishEndpoint publishEndpoint)
        {
            _requestClient = requestClient;
            _provider = provider;
            _checkOrderClient = checkOrderClient;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var (found, notFound) =
                await _checkOrderClient.GetResponse<OrderStatus, OrderNotFound>(new CheckOrder() {OrderId = id});
            if (found.IsCompletedSuccessfully)
            {
                return Ok((await found).Message);
            }

            return NotFound((await notFound).Message);
        }

        [HttpPut]
        public async Task<IActionResult> Put(Guid id, string customerNumber)
        {
            var endpoint = await _provider.GetSendEndpoint(
                new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
            await endpoint.Send(new SubmitOrder(id,InVar.Timestamp,customerNumber));
            return Accepted();
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid id, string customerNumber)
        {
            var (accepted, rejected) =
                await _requestClient.GetResponse<OrderSubmissionAccepted, OrderSubmissionRejected>(new SubmitOrder(id,InVar.Timestamp, customerNumber));
            if (accepted.IsCompletedSuccessfully)
                return Ok((await accepted).Message);
            return BadRequest(rejected.Result.Message);
        }

        [HttpPatch]
        public async Task<IActionResult> Patch(Guid orderId)
        {
            await _publishEndpoint.Publish<OrderAccepted>(
                new {OrderId = orderId, InVar.Timestamp});
            return Ok();
        }
    }
}