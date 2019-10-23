namespace EventFly.Domain.Aggregates
{
    public interface IAggregateState<TIdentity> 
    {
        string Id { get; set; }
    }
}