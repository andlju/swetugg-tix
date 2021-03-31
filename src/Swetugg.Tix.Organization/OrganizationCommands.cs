using Dapper;
using Swetugg.Tix.Organization.Contract;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.Organization
{
    public class OrganizationCommands : IOrganizationCommands
    {
        private readonly string _connectionString;

        public OrganizationCommands(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task CreateOrganization(OrganizationInfo info, Guid userId)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[Organization] " +
                    "(OrganizationId, Name, LastUpdated) " +
                    "VALUES (@OrganizationId, @Name, SYSUTCDATETIME()) ", new { 
                        info.OrganizationId, 
                        info.Name
                    });

                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[OrganizationUser] " +
                    "(OrganizationId, UserId, LastUpdated) " +
                    "VALUES (@OrganizationId, @UserId, SYSUTCDATETIME()) ", new
                    {
                        info.OrganizationId,
                        UserId = userId
                    });
                trans.Complete();
            }
        }

        public async Task LinkUserToOrganization(Guid organizationId, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidOperationException("UserId cannot be empty");
            if (organizationId == Guid.Empty)
                throw new InvalidOperationException("OrganizationId cannot be empty");

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[OrganizationUser] " +
                    "(OrganizationId, UserId, LastUpdated) " +
                    "VALUES (@OrganizationId, @UserId, SYSUTCDATETIME())", new { organizationId, userId });

                trans.Complete();
            }

        }

        public async Task RemoveUserFromOrganization(Guid organizationId, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidOperationException("UserId cannot be empty");
            if (organizationId == Guid.Empty)
                throw new InvalidOperationException("OrganizationId cannot be empty");

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "DELETE FROM [Access].[OrganizationUser] " +
                    "WHERE OrganizationId = @OrganizationId " +
                    "AND UserId = @UserId ", new { organizationId, userId });

                trans.Complete();
            }
        }
    }
}
