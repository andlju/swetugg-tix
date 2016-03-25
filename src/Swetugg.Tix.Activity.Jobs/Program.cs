using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Swetugg.Tix.Activity.Commands;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using NEventStore;
using Swetugg.Tix.Activity.Domain;

namespace Swetugg.Tix.Activity.Jobs
{
    public class Program
    {
        public static Assembly CommandAssembly = typeof(CreateActivity).Assembly;

        public static DomainHost _domainHost;

        public static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json");
            configBuilder.AddEnvironmentVariables();
            configBuilder.AddUserSecrets();

            var config = configBuilder.Build();

            // Setup an InMemory EventStore 
            var eventStoreWireup = Wireup.Init()
                .UsingInMemoryPersistence();

            // Build the domain host
            _domainHost = DomainHost.Build(eventStoreWireup);

            JobHostConfiguration jobHostConfig =
                new JobHostConfiguration(config["Data:AzureWebJobsStorage:ConnectionString"]);
            jobHostConfig.UseServiceBus(new ServiceBusConfiguration()
            {
                ConnectionString = config["Data:AzureServiceBus:ConnectionString"]
            });
            JobHost host = new JobHost(jobHostConfig);
            host.RunAndBlock();
        }

        public static void DispatchCommand([ServiceBusTrigger("activitycommands")] BrokeredMessage commandMsg)
        {
            var messageType = CommandAssembly.GetType(commandMsg.Label, false);
            if (messageType == null)
            {
                throw new InvalidOperationException($"Unknown message type '{commandMsg.Label}'");
            }
            var command = JsonConvert.DeserializeObject(commandMsg.GetBody<string>(), messageType);

            Console.Out.WriteLine($"Dispatching {messageType.Name} command");
            _domainHost.Dispatcher.Dispatch(command);
            Console.Out.WriteLine("Command handled successfully");
        }
    }
}
