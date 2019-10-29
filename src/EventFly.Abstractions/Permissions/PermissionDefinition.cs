
using System;
using EventFly.ValueObjects;

namespace EventFly.Permissions
{
    public class PermissionDefinition : ValueObject, IPermissionDefinition
    {
        public PermissionDefinition(string permissionCode)
        {
            PermissionCode = permissionCode;
        }
        public PermissionDefinition(Type targetAggregateType, string permissionCode)
        {
            TargetAggregateType = targetAggregateType;
            PermissionCode = permissionCode;
        }

        public Type TargetAggregateType { get; }

        public string PermissionCode { get; }

    }
}