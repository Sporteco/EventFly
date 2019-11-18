using EventFly.Core;
using EventFly.Definitions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace EventFly.Permissions
{
    internal sealed class SecurityService : ISecurityService
    {
        private readonly IPermissionProvider _permissionProvider;
        private readonly IApplicationDefinition _definition;

        public SecurityService(IServiceProvider serviceProvider, IApplicationDefinition definition)
        {
            _permissionProvider = serviceProvider.GetService<IPermissionProvider>();
            _definition = definition;
        }
        public void Authorized(String userId)
        {
            if (String.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
        }

        public void HasPermissions(String userId, IIdentity targetObjectId, params PermissionCode[] permissionCodes)
        {
            if (permissionCodes == null || !permissionCodes.Any()) return;

            Authorized(userId);

            if (!CheckPermissions(userId, targetObjectId, permissionCodes))
                throw new UnauthorizedAccessException();
        }

        public Boolean CheckPermissions(String userId, IIdentity targetObjectId, params PermissionCode[] permissionCodes)
        {
            if (_permissionProvider == null)
                throw new InvalidOperationException("IPermissionProvider not registered.");

            var codes = permissionCodes.ToList();
            var permissions = _definition.Permissions.Where(i => codes.Contains(i.PermissionCode)).ToList();

            var targetObjectType = targetObjectId?.GetType();

            foreach (var permission in permissions)
            {
                //The permission to object, the type of object and identifier must match
                if (permission.TargetAggregateType != null && targetObjectType != permission.TargetAggregateType)
                    return false;
            }

            var userPermissions = targetObjectId != null ? _permissionProvider.GetUserPermissions(userId, targetObjectId.Value).Distinct()
                : _permissionProvider.GetUserPermissions(userId, null).Distinct();

            var existsPermissions = userPermissions.Where(i => codes.Contains(i.PermissionCode)).Select(i => i.PermissionCode).Distinct();

            if (existsPermissions.Count() != codes.Count) return false;


            return true;
        }
    }
}