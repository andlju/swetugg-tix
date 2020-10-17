using Swetugg.Tix.Activity.Content.Contract;
using System;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Swetugg.Tix.Activity.Content
{

    public class SqlActivityContentQuery : IActivityContentQuery
    {
        private readonly string _connectionString;

        public SqlActivityContentQuery(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<ActivityContent> GetActivity(Guid activityId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var result = await conn.QuerySingleAsync<ActivityContent>(
                    "SELECT Name FROM [ActivityContent].[Activity] " +
                    "WHERE ActivityId = @ActivityId", new
                    {
                        ActivityId = activityId
                    });

                return result;
            }
        }
        public async Task<IEnumerable<TicketTypeContent>> GetTicketTypes(Guid activityId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var result = await conn.QueryAsync<TicketTypeContent>(
                    "SELECT Name FROM [ActivityContent].[TicketType] " +
                    "WHERE ActivityId = @ActivityId", new
                    {
                        ActivityId = activityId
                    });

                return result;
            }
        }
    }
}
