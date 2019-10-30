using EventFly.Commands;

namespace Demo.Commands
{
    public class TrackUserTouchingCommand : Command<UserId>
    {
        public TrackUserTouchingCommand(UserId aggregateId) : base(aggregateId) { }
    }
}