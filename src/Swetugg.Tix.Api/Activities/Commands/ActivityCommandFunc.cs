using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Api.Authorization;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public abstract class ActivityCommandFunc<TCommand> : AuthorizedFunc<TCommand>
        where TCommand : ActivityCommand, new()
    {
        protected static string[] acceptedScopes = new[] { "access_as_backoffice" };
        private readonly IMessageSender _sender;

        protected ActivityCommandFunc(IActivityCommandMessageSender sender, IAuthManager authManager) : base(authManager)
        {
            _sender = sender;
        }

        protected void OverrideProperties(TCommand cmd, object overrides)
        {
            var overridesType = overrides.GetType();
            foreach (var prop in overridesType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var cmdProp = cmd.GetType().GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (cmdProp == null)
                    throw new InvalidOperationException($"Couldn't find property with name {prop.Name} to override");
                cmdProp.SetValue(cmd, prop.GetValue(overrides));
            }
        }

        protected async Task<(IActionResult, TCommand)> ProcessCommand(HttpRequest req, ILogger log, object overrides)
        {
            TCommand cmd;
            if (req.ContentLength > 0)
            {
                cmd = await JsonSerializer.DeserializeAsync<TCommand>(req.Body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            else
            {
                cmd = new TCommand();
            }

            var commandId = Guid.NewGuid();
            cmd.CommandId = commandId;
            if (req.Query.TryGetValue("OwnerId", out var ownerId))
                cmd.OwnerId = Guid.Parse(ownerId);

            if (overrides != null)
            {
                OverrideProperties(cmd, overrides);
            }
            return (await Process(req, log, cmd), cmd);
        }

        protected override async Task<TCommand> HandleUser(HttpRequest req, ILogger log, TCommand cmd)
        {
            var user = await AuthManager.GetAuthorizedUser();
            
            // If no user is logged in, you should not be allowed to send an activity command
            if (user.UserId == null)
                return null;

            // If no Owner has been set, default to the current user
            if (cmd.OwnerId == Guid.Empty)
                cmd.OwnerId = user.UserId.Value;
            cmd.Headers.UserId = user.UserId.Value;
            return cmd;
        }

        protected override async Task<IActionResult> HandleRequest(HttpRequest req, ILogger log, TCommand cmd)
        {
            if (cmd == null)
            {
                // No command body - probably because the user was not found.
                return new BadRequestResult();
            }

            await _sender.Send(cmd);

            return new OkObjectResult(new { activityId = cmd.ActivityId, ownerId = cmd.OwnerId, commandId = cmd.CommandId });
        }
    }
}
