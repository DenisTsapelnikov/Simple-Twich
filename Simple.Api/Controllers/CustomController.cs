using System;
using System.Threading.Tasks;
using Components.Consumers;
using Contracts;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Simple_Twich.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomController : Controller
    {
        private readonly IPublishEndpoint _publish;

        public CustomController(IPublishEndpoint publish)
        {
            _publish = publish;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id, string customerNumber)
        {
            await _publish.Publish(new CustomAccountClosed(id, customerNumber));
            
            return Ok();
        }
    }
}