using EventFly.Commands;
using Demo.ValueObjects;

namespace Demo.Commands
{
    public class RenameUserCommand : Command<UserId>
    {
        public  UserName UserName { get; }

        public RenameUserCommand(UserId aggregateId, UserName userName) : base(aggregateId)
        {
            UserName = userName;
        }
    }
}