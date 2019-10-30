using Demo.Commands;
using Demo.Events;
using EventFly.Aggregates;
using EventFly.Domain;
using System.Threading.Tasks;

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