using Dapper;
using Swetugg.Tix.User.Contract;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;

namespace Swetugg.Tix.User
{

    public class UserCommands : IUserCommands
    {
        private readonly string _connectionString;

        public UserCommands(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task SetUser(UserInfo info)
        {
            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "UPDATE [Access].[User] " +
                    "SET Name              = @Name " +
                    ",   LastUpdated       = SYSUTCDATETIME() " +
                    "WHERE UserId = @UserId", info);

                trans.Complete();
            }
        }

        public async Task<Guid> CreateUser(UserInfo info)
        {
            if (info.UserId != null)
            {
                throw new InvalidOperationException("Don't set a UserId when creating a new user");
            }
            var newUserId = Guid.NewGuid();
            info.UserId = Guid.NewGuid();
            info.Status = UserStatus.Created;

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[User] " +
                    "(UserId, Name, Status, LastUpdated) " +
                    "VALUES (@UserId, @Name, @Status, SYSUTCDATETIME()) ", info);

                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[UserLogin] " +
                    "(IssuerId, Subject, UserId, LastUpdated) " +
                    "SELECT i.IssuerId, @Subject, @UserId, SYSUTCDATETIME() FROM [Access].[Issuer] i WHERE i.IssuerIdentifier = @IssuerIdentifier ", info
                    );

                trans.Complete();
            }
            return newUserId;
        }

        public async Task<Guid> AddUserRole(Guid userId, Guid roleId, IEnumerable<UserRoleAttribute> attributes)
        {
            if (userId == Guid.Empty)
                throw new InvalidOperationException("UserId cannot be empty");
            if (roleId == Guid.Empty)
                throw new InvalidOperationException("RoleId cannot be empty");

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                var userRoleId = Guid.NewGuid();
                await conn.ExecuteAsync(
                    "INSERT INTO [Access].[UserRole] " +
                    "(UserRoleId, UserId, RoleId, LastUpdated) " +
                    "VALUES (@UserRoleId, @UserId, @RoleId, SYSUTCDATETIME()) ", new { userRoleId, userId, roleId });

                foreach (var attribute in attributes)
                {
                    var userRoleAttributeId = Guid.NewGuid();
                    await conn.ExecuteAsync(
                        "INSERT INTO [Access].[UserRoleAttribute] " +
                        "(UserRoleAttributeId, UserRoleId, Attribute, Value) " +
                        "VALUES (@UserRoleAttributeId, @UserRoleId, @Attribute, @Value) ", new { userRoleAttributeId, userRoleId, Attribute = attribute.Name, attribute.Value });
                }
                trans.Complete();
                return userRoleId;
            }
        }

        public async Task DeleteUserRole(Guid userRoleId)
        {
            if (userRoleId == Guid.Empty)
                throw new InvalidOperationException("UserRoleId cannot be empty");

            using (var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            using (var conn = new SqlConnection(_connectionString))
            {
                await conn.ExecuteAsync(
                    "DELETE FROM [Access].[UserRoleAttribute] " +
                    "WHERE UserRoleId = @UserRoleId ", new { userRoleId });

                await conn.ExecuteAsync(
                    "DELETE FROM [Access].[UserRole] " +
                    "WHERE UserRoleId = @UserRoleId ", new { userRoleId });

                trans.Complete();
            }
        }
    }
}
