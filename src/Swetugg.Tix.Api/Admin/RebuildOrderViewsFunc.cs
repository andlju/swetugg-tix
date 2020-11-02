using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Order.Commands.Admin;
using Swetugg.Tix.Api.Orders.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Admin
{

    public class RebuildOrderViewsFunc : OrderAdminCommandFunc<RebuildViews>
    {
        public RebuildOrderViewsFunc(IOrderCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("RebuildOrderViews")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders-admin/{orderId}/rebuild")]
            HttpRequest req,
            Guid orderId,
            ILogger log)
        {
            var cmd = await Process(req, new { orderId }, log);

            return new OkObjectResult(new { orderId, commandId = cmd.CommandId });
        }
    }
}
