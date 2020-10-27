using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swetugg.Tix.Api.Models;
using Swetugg.Tix.Api.Options;
using Swetugg.Tix.Order.Views;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Swetugg.Tix.Api.Orders
{

    public class GetOrderFunc
    {
        private readonly string _connectionString;
        public GetOrderFunc(IOptions<ApiOptions> options)
        {
            _connectionString = options.Value.ViewsDbConnection;
        }

        [FunctionName("GetOrder")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "orders/{orderId}")]
            HttpRequest req,
            string orderId,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            using (var conn = new SqlConnection(_connectionString))
            {
                var activity = await conn.QuerySingleOrDefaultAsync<OrderView>(
                    "SELECT o.OrderId, o.Revision " +
                    "FROM OrderViews.OrderView o " +
                    "WHERE o.OrderId = @OrderId",
                    new { orderId });
                if (activity != null)
                {
                    return new OkObjectResult(activity);
                }
            }
            return new NotFoundResult();
        }
    }
}
