using EventFly.Commands;
using Demo.ValueObjects;
using EventFly.Security;

namespace Demo.User
{
    [HasPermissions(DemoContext.ChangeUser)]
    public class RenameUserCommand : Command<UserId>
    {
        public  UserName UserName { get; }

        public RenameUserCommand(UserId aggregateId, UserName userName) : base(aggregateId)
        {
            UserName = userName;
        }
    }
}