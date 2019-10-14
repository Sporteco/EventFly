using System.ComponentModel;
using Akkatecture.Commands;
using Demo.ValueObjects;

namespace Demo.Commands
{
    [Description("Создание нового пользователя")]
    public class CreateUserCommand : Command<UserId>
    {
        public readonly UserName UserName;
        public readonly Birth Birth;

        public CreateUserCommand(UserId userId, UserName userName, Birth birth) : base(userId)
        {
            UserName = userName;
            Birth = birth;
        }
    }
}