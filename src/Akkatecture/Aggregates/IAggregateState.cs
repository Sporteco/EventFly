namespace Akkatecture.Aggregates
{
    public interface IAggregateState<TIdentity> 
    {
        string Id { get; set; }
    }
}