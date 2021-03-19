using Dapper;
using Swetugg.Tix.User.Contract;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Swetugg.Tix.User
{
    public class UserQueries : IUserQueries
    {
        private readonly string _connectionString;

        public UserQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<UserInfo> GetUser(Guid userId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var userInfo = (await conn.QueryFirstOrDefaultAsync<UserInfo>(
                    "SELECT u.UserId, u.Name, u.Status " +
                    "FROM [Access].[User] u " +
                    "WHERE u.UserId = @UserId AND u.Status <> @DeletedStatus",
                    new { UserId = userId, DeletedStatus = UserStatus.Deleted }));

                return userInfo;
            }
        }

        public async Task<UserInfo> GetUserFromLogin(string subject, string issuer)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var userInfo = (await conn.QueryFirstOrDefaultAsync<UserInfo>(
                    "SELECT u.UserId, u.Name, u.Status " +
                    "FROM [Access].[User] u JOIN [Access].[UserLogin] ul ON ul.UserId = u.UserId JOIN [Access].[Issuer] i ON i.IssuerId = ul.IssuerId " +
                    "WHERE ul.Subject = @Subject AND i.IssuerIdentifier = @Issuer AND u.Status <> @DeletedStatus",
                    new { Subject = subject, Issuer = issuer, DeletedStatus = UserStatus.Deleted }));

                return userInfo;
            }
        }
    }
}
