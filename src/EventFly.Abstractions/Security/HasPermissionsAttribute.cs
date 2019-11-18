using EventFly.Permissions;
using System;
using System.Linq;

namespace EventFly.Security
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HasPermissionsAttribute : Attribute
    {
        public PermissionCode[] Permissions { get; }

        public HasPermissionsAttribute(params String[] permissionCodes)
        {
            Permissions = permissionCodes.Select(i => new PermissionCode(i)).ToArray();
        }
    }

}