using Akkatecture.Commands;
using Demo.ValueObjects;

namespace Demo.Commands
{
    public class RenameUserCommand : Command<UserId>
    {
        public readonly UserName UserName;

        public RenameUserCommand(UserId userId, UserName userName) : base(userId)
        {
            UserName = userName;
        }
    }
}