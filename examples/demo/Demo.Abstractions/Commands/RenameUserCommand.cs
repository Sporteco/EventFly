using Akkatecture.Commands;
using Demo.ValueObjects;

namespace Demo.Commands
{
    public class RenameUserCommand : Command<UserId>
    {
        public readonly UserName UserName;

        public RenameUserCommand(UserId aggregateId, UserName userName) : base(aggregateId)
        {
            UserName = userName;
        }
    }
}