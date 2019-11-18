
using EventFly.ValueObjects;
using System;

namespace EventFly.Permissions
{
    public class PermissionDefinition : ValueObject, IPermissionDefinition
    {
        public PermissionDefinition(String permissionCode)
        {
            PermissionCode = permissionCode;
        }
        public PermissionDefinition(Type targetAggregateType, String permissionCode)
        {
            TargetAggregateType = targetAggregateType;
            PermissionCode = permissionCode;
        }

        public Type TargetAggregateType { get; }

        public String PermissionCode { get; }

    }
}