namespace EventFly.ValueObjects
{
    public class ValueObjectWithCode<TCode> : ValueObject
    where TCode : SingleValueObject<System.String>
    {
        public TCode Code { get; set; }
    }
}
