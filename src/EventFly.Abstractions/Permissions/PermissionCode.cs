using EventFly.ValueObjects;

namespace EventFly.Permissions
{
    public class PermissionCode : SingleValueObject<System.String>
    {
        public PermissionCode(System.String value) : base(value) { }

        public static implicit operator PermissionCode(System.String value)
        {
            return new PermissionCode(value);
        }
    }
}