using Dapper;
using Swetugg.Tix.Organization.Contract;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Swetugg.Tix.Organization
{
    public class OrganizationQueries : IOrganizationQueries
    {
        private readonly string _connectionString;

        public OrganizationQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<OrganizationInfo[]> ListOrganizations(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var organizations = (await conn.QueryAsync<OrganizationInfo>(
                    "SELECT o.OrganizationId, o.Name " +
                    "FROM [Access].[Organization] o " +
                    "JOIN [Access].[OrganizationUser] ou ON o.OrganizationId = ou.OrganizationId " +
                    "WHERE ou.UserId = @UserId ",
                    new { UserId = userId }));

                return organizations.ToArray();
            }
        }

    }
}
