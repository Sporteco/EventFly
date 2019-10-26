using System.Collections.Generic;
using System.ComponentModel;
using EventFly.Queries;

namespace Demo.Queries
{
    [Description("Информация о пользователе")]
    public class UserInfo
    {
        [Description("ID")]
        public string Id { get; set; }


        [Description("Имя пользователя")]
        public string Name { get; set; }
    }

    [Description("Список пользвоателей")]

    public class UsersResult
    {
        public UsersResult(ICollection<UserInfo> items, int total)
        {
            Items = items;
            Total = total;
        }
        [Description("Пользователи")]

        public ICollection<UserInfo> Items { get; private set; }

        [Description("Количество")]

        public int Total { get; private set; }
    }
    [Description("Запрос мнформации о пользователях")]

    public class UsersQuery : IQuery<UsersResult>
    {
        [Description("Искомое слово в имени пользователя")]
        public string NameFilter { get; set; }

        public IEnumerable<UserInfo> Slots { get; set; }

    }

    public class User1Query : IQuery<UsersResult>
    {
        public UserId Id { get; set; }
    }
    public class User2Query : IQuery<UsersResult>
    {
        public UserId Id { get; set; }
    }

}

