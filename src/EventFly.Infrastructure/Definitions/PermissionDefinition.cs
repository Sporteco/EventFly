
using System;

namespace EventFly.Definitions
{
    internal class PermissionDefinition : IPermissionDefinition
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

        public override int GetHashCode() => PermissionCode.GetHashCode() + (TargetAggregateType == null ? 0 : TargetAggregateType.GetHashCode());
    }
}