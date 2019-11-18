using Demo.User.Commands;
using Demo.User.Events;
using EventFly.Aggregates;
using EventFly.DomainService;
using System.Threading.Tasks;

namespace Demo.Domain.Services
{
    public class UserTouchTrackingService : AsynchronousDomainService<UserTouchTrackingService>,
        IDomainServiceIsStartedByAsync<UserId, UserNotesChangedEvent>
    {
        public Task HandleAsync(IDomainEvent<UserId, UserNotesChangedEvent> domainEvent)
        {
            return PublishCommandAsync(new TrackUserTouchingCommand(domainEvent.AggregateIdentity));
        }
    }
}