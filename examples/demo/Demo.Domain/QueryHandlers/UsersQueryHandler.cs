using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Akkatecture.Queries;
using Dapper;
using Demo.Queries;

namespace Demo.Domain.QueryHandlers
{
    public class UsersQueryHandler : QueryHandler<UsersQuery, ICollection<UserInfo>>
    {
        private ICollection<UserInfo> _cache;

        public override ICollection<UserInfo> ExecuteQuery(UsersQuery query)
        {
            if (_cache == null)
            {
                using (var db = new SqlConnection(DbHelper.ConnectionString))
                {
                    _cache = db.Query<UserInfo>("SELECT Id, Name FROM [User]").ToList();
                }
            }

            return _cache;
        }

    }
}