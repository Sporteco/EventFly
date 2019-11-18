using System;

namespace EventFly.Permissions
{
    public interface IPermissionDefinition
    {
        String PermissionCode { get; }
        Type TargetAggregateType { get; }
    }
}