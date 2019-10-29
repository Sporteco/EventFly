using System;

namespace EventFly.Security
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HasPermissionsAttribute : Attribute
    {
        public string[] Permissions { get; }
        
        public HasPermissionsAttribute(params string[] permissionCodes)
        {
            Permissions = permissionCodes;
        }
    }

}