using EventFly.Commands;

namespace Demo.User.Commands
{
    public class TrackUserTouchingCommand : Command<UserId>
    {
        public TrackUserTouchingCommand(UserId aggregateId) : base(aggregateId) { }
    }
}