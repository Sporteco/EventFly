using EventFly.ValueObjects;

namespace EventFly.Permissions
{
    public class PermissionCode : SingleValueObject<string>
    {
        public PermissionCode(string value) : base(value){}

        public static implicit operator PermissionCode(string value)
        {
            return new PermissionCode(value);
        }
    }
}