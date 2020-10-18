using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Ticket.Commands;
using System;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Tickets.Commands
{
    public class CreateTicketFunc : TicketCommandFunc<CreateTicket>
    {
        public CreateTicketFunc(ITicketCommandMessageSender sender) : base(sender)
        {
        }

        [FunctionName("CreateTicket")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tickets")]
            HttpRequest req,
            ILogger log)
        {
            var ticketId = Guid.NewGuid();
            var cmd = await Process(req, new { ticketId }, log);

            return new OkObjectResult(new { ticketId, commandId = cmd.CommandId });
        }
    }
}
