namespace Akkatecture.Aggregates
{
    public interface IAggregateState<TIdentity> 
    {
        TIdentity Id { get; set; }
    }
}