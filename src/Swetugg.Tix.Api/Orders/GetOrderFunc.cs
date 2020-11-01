using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Options;
using Swetugg.Tix.Order.Views;
using Swetugg.Tix.Order.Views.TableStorage;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Orders
{

    public class GetOrderFunc
    {
        private readonly TableStorageViewReader _viewReader;

        public GetOrderFunc(IOptions<ApiOptions> options)
        {
            // TODO Use Singleton
            _viewReader = new TableStorageViewReader(options.Value.AzureWebJobsStorage, "orderview");
        }

        [FunctionName("GetOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{orderId}")]
            HttpRequest req,
            string orderId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var order = await _viewReader.GetEntity<OrderViewEntity, OrderView>(orderId, orderId);

            if (order != null)
            {
                return new OkObjectResult(order);
            }
            return new NotFoundResult();
        }
    }
}
