using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Order.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Orders.Commands
{
    public class CreateOrderFunc : OrderCommandFunc<CreateOrderWithTickets>
    {
        public CreateOrderFunc(IOrderCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("CreateOrderWithTickets")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders")]
            HttpRequest req,
            ILogger log)
        {
            var orderId = Guid.NewGuid();
            var cmd = await Process(req, new { orderId }, log);

            return new OkObjectResult(new { orderId, commandId = cmd.CommandId });
        }
    }
}
