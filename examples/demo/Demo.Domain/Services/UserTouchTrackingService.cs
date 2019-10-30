using Demo.Commands;
using Demo.Events;
using EventFly.Aggregates;
using System.Threading.Tasks;
using EventFly.DomainService;

namespace Demo.Domain.Services
{
    public class UserTouchTrackingService : DomainService<UserTouchTrackingService>,
        IDomainServiceIsStartedByAsync<UserId, UserNotesChangedEvent>
    {
        public Task HandleAsync(IDomainEvent<UserId, UserNotesChangedEvent> domainEvent)
        {
            return PublishCommandAsync(new TrackUserTouchingCommand(domainEvent.AggregateIdentity));
        }
    }
}