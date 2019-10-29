using System;

namespace EventFly.Permissions
{
    public interface IPermissionDefinition
    {
        string PermissionCode { get; }
        Type TargetAggregateType  { get; }
    }
}