using System.Collections.Generic;
using System.Linq;
using Akkatecture.Queries;
using Demo.Queries;

namespace Demo.Db.QueryHandlers
{
    public class UsersQueryHandler : QueryHandler<UsersQuery, ICollection<UserInfo>>
    {
        private ICollection<UserInfo> _cache;

        public override ICollection<UserInfo> ExecuteQuery(UsersQuery query)
        {
            if (_cache == null)
            {
                using (var db = new TestDbContext())
                {
                    _cache = db.User.Select(i => new UserInfo(i.Id.Value, i.Name.Value)).ToList();
                }
            }

            return _cache;
        }

    }
}