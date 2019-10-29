using System;

namespace EventFly.Security
{
    public class HasPermissionsAttribute : Attribute
    {
        public string[] Permissions { get; }

        public HasPermissionsAttribute(params string[] permissions)
        {
            Permissions = permissions;
        }

    }
}