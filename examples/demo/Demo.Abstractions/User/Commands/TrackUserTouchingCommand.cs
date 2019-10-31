using EventFly.Commands;

namespace Demo.User
{
    public class TrackUserTouchingCommand : Command<UserId>
    {
        public TrackUserTouchingCommand(UserId aggregateId) : base(aggregateId) { }
    }
}