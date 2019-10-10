using System.Collections.Generic;
using Akkatecture.Queries;

namespace Demo.Queries
{
    public class UserInfo
    {
        public UserInfo(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
    public class UsersQuery : IQuery<ICollection<UserInfo>>
    {
        public UsersQuery(string nameFilter)
        {
            NameFilter = nameFilter;
        }

        public string NameFilter { get; }
    }
}
