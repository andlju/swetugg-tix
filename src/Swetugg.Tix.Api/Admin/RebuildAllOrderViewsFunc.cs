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

    public class RebuildAllOrderViewsFunc : OrderAdminCommandFunc<RebuildAllViews>
    {
        public RebuildAllOrderViewsFunc(IOrderCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("RebuildAllOrderViews")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "orders-admin/rebuild")]
            HttpRequest req,
            ILogger log)
        {
            var cmd = await Process(req, new { }, log);

            return new OkObjectResult(new { commandId = cmd.CommandId });
        }
    }
}
