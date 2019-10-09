using Akkatecture.ValueObjects;

namespace Demo.ValueObjects
{
    public class UserName : SingleValueObject<string>
    {
        public UserName(string value) : base(value){}
    }
}