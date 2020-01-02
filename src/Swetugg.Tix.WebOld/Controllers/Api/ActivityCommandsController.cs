using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swetugg.Tix.Activity.Commands;

namespace Swetugg.Tix.Web.Controllers.Api
{

    [Route("api/activity/commands")]
    public class ActivityCommandsController : Controller
    {
        private readonly IMessageSender _sender;

        public ActivityCommandsController(IMessageSender sender)
        {
            _sender = sender;
        }

        [HttpGet("test")]
        public async Task<string> Get()
        {
            var createActivityCommand = new CreateActivity()
            {
                ActivityId = Guid.NewGuid(),
                CommandId = Guid.NewGuid()
            };

            await _sender.Send(createActivityCommand);

            return "Command sent";
        }
    }
}