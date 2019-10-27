using System;
using System.Collections.Generic;
using System.Linq;
using EventFly.Queries;
using Demo.Queries;
using EventFly.ReadModels;
using System.Threading.Tasks;
using Demo.Infrastructure.ReadModels;

namespace Demo.Infrastructure.QueryHandlers
{
    public class UsersQueryHandler : QueryHandler<UsersQuery, UsersResult>
    {
        private readonly IQueryableReadModelStorage<UsersInfoReadModel> _storage;

        public UsersQueryHandler(IReadModelStorage<UsersInfoReadModel> storage)
        {
            _storage = storage as IQueryableReadModelStorage<UsersInfoReadModel>;
        }
        public override Task<UsersResult> ExecuteQuery(UsersQuery query)
        {
            var items = _storage?.Items
                //.Where(i => query.NameFilter == null || i.UserName.StartsWith(query.NameFilter, StringComparison.InvariantCultureIgnoreCase))
                .Select(i => new UserInfo {Id = i.Id, Name = i.UserName}).ToList();

            return Task.FromResult(new UsersResult(items, 100));
        }
    }

    public class UsersQuery1Handler : QueryHandler<User1Query, UsersResult>
    {
        public override Task<UsersResult> ExecuteQuery(User1Query query)
        {
            var res =  new UsersResult(new List<UserInfo>
            {
                new UserInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Name"
                },
                new UserInfo
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Name2"
                }
            },100);
            return Task.FromResult(res);
        }
    }

    public class UsersQuery2Handler : QueryHandler<User2Query, UsersResult>
    {
        public override Task<UsersResult> ExecuteQuery(User2Query query)
        {
            throw new NotImplementedException();
        }
    }

}
