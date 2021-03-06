﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Tix.Activity.Commands.Admin;
using Swetugg.Tix.Api.Activities.Commands;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Admin
{
    public abstract class ActivityAdminCommandFunc<TCommand> where TCommand : ActivityAdminCommand, new()
    {
        private readonly IMessageSender _sender;

        protected ActivityAdminCommandFunc(IActivityCommandMessageSender sender)
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
            log.LogInformation("C# HTTP trigger function processed a request.");

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
