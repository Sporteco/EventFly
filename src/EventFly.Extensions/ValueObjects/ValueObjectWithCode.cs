using EventFly.ValueObjects;

namespace EventFly.Extensions.ValueObjects
{
    public class ValueObjectWithCode<TCode> : ValueObject
    where TCode : SingleValueObject<string>
    {
        public TCode Code { get; set; }
    }
}
