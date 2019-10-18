using System.Data.SqlClient;
using System.Linq;
using EventFly.Queries;
using Dapper;
using Demo.Queries;
using Microsoft.Data.Sqlite;

namespace Demo.Domain.QueryHandlers
{
    public class UsersQueryHandler : QueryHandler<UsersQuery, UsersResult>
    {
        public override UsersResult ExecuteQuery(UsersQuery query)
        {
            var filter = "";
            if (!string.IsNullOrEmpty(query.NameFilter))
                filter = $"WHERE Name LIKE '%{query.NameFilter}%'";

            using (var db = new SqliteConnection(DbHelper.ConnectionStringSqLite))
            {
                return new UsersResult(
                    db.Query<UserInfo>($"SELECT Id, Name FROM [User] {filter}").ToList(),
                    100);
            }
        }
    }
}