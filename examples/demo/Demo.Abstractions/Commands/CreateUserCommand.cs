using System.ComponentModel;
using EventFly.Commands;
using Demo.ValueObjects;

namespace Demo.Commands
{
    [Description("Создание нового пользователя")]
    public class CreateUserCommand : Command<UserId>
    {
        public UserName UserName { get; }
        public Birth Birth { get; }

        public CreateUserCommand(UserId aggregateId, UserName userName, Birth birth) : base(aggregateId)
        {
            UserName = userName;
            Birth = birth;
        }
    }
}