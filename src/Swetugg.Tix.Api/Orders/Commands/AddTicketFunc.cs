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
    public class AddTicketFunc : OrderCommandFunc<AddTickets>
    {
        public AddTicketFunc(IOrderCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("AddTickets")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders/{orderId}/tickets")]
            HttpRequest req,
            Guid orderId,
            ILogger log)
        {
            var cmd = await Process(req, new { orderId }, log);

            return new OkObjectResult(new { orderId, commandId = cmd.CommandId });
        }
    }
}
