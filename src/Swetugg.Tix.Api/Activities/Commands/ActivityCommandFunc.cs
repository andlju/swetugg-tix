using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Swetugg.Tix.Activity.Commands;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Activities.Commands
{
    public abstract class ActivityCommandFunc<TCommand> where TCommand : ActivityCommand, new()
    {
        protected static string[] acceptedScopes = new[] { "access_as_admin" };
        private readonly IMessageSender _sender;

        protected ActivityCommandFunc(IActivityCommandMessageSender sender)
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

        protected async Task<TCommand> Process(HttpRequest req, object overrides, ILogger log)
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
            if (overrides != null)
            {
                OverrideProperties(cmd, overrides);
            }

            await _sender.Send(cmd);

            return cmd;
        }
    }
}
