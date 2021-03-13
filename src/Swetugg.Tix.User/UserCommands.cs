using Dapper;
using Swetugg.Tix.User.Contract;
using System;
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
                    "UPDATE [Users].[User] " +
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
                    "INSERT INTO [Users].[User] " + 
                    "(UserId, Name, Status, LastUpdated) " +
                    "VALUES (@UserId, @Name, @Status, SYSUTCDATETIME()) ", info);

                await conn.ExecuteAsync(
                    "INSERT INTO [Users].[UserLogin] " +
                    "(IssuerId, Subject, UserId, LastUpdated) " +
                    "SELECT i.IssuerId, @Subject, @UserId, SYSUTCDATETIME() FROM [Users].[Issuer] i WHERE i.IssuerIdentifier = @IssuerIdentifier ", info
                    );

                trans.Complete();
            }
            return newUserId;
        }
    }
}
