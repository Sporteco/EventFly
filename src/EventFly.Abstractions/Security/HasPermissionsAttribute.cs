using System;
using System.Linq;
using EventFly.Permissions;

namespace EventFly.Security
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HasPermissionsAttribute : Attribute
    {
        public PermissionCode[] Permissions { get; }
        
        public HasPermissionsAttribute(params string[] permissionCodes)
        {
            Permissions = permissionCodes.Select(i=>new PermissionCode(i)).ToArray();
        }
    }

}