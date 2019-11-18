using EventFly.Aggregates;

namespace EventFly.ReadModels
{
    public interface IReadModel
    {
        System.String Id { get; }
        void ApplyEvent(IDomainEvent e);
    }
}