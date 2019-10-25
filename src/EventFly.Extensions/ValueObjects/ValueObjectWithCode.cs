namespace EventFly.ValueObjects
{
    public class ValueObjectWithCode<TCode> : ValueObject
    where TCode : SingleValueObject<string>
    {
        public TCode Code { get; set; }
    }
}
