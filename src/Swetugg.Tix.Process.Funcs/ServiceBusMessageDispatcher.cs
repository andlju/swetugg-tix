using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Swetugg.Tix.Activity.Commands;
using Swetugg.Tix.Infrastructure;
using Swetugg.Tix.Process.Funcs.Options;
using Swetugg.Tix.Order.Commands;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Swetugg.Tix.Process.Funcs
{
  public class ServiceBusMessageDispatcher : ISagaMessageDispatcher
  {
    private readonly string _serviceBusConnectionString;

    private IDictionary<Assembly, QueueClient> _clients = new Dictionary<Assembly, QueueClient>();

    public ServiceBusMessageDispatcher(IOptions<ProcessOptions> processOptions)
    {
      var activityQueueName = processOptions.Value.ActivityCommandsQueue;
      var orderQueueName = processOptions.Value.OrderCommandsQueue;
      _serviceBusConnectionString = processOptions.Value.TixServiceBus;
      RegisterAssemblyQueue(typeof(CreateOrder).Assembly, orderQueueName);
      RegisterAssemblyQueue(typeof(CreateActivity).Assembly, activityQueueName);
    }

    public void RegisterAssemblyQueue(Assembly assembly, string queueName)
    {
      _clients.Add(assembly, new QueueClient(_serviceBusConnectionString, queueName));
    }

    public async Task Dispatch(object cmd)
    {
      if (!_clients.TryGetValue(cmd.GetType().Assembly, out var client))
      {
        return;
      }
      var byteBody = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(cmd));
      var message = new Message(byteBody)
      {
        Label = cmd.GetType().FullName
      };

      await client.SendAsync(message);
    }
  }
}