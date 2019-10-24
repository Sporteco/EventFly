namespace EventFly.Aggregates
{
    public interface IAggregateState<TIdentity> 
    {
        string Id { get; set; }
    }
}