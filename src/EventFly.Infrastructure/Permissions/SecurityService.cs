using System;
using System.Linq;
using EventFly.Core;
using EventFly.Definitions;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.Permissions
{
    internal sealed  class SecurityService : ISecurityService
    {
        private readonly IPermissionProvider _permissionProvider;
        private readonly IApplicationDefinition _definition;

        public SecurityService(IServiceProvider serviceProvider, IApplicationDefinition definition)
        {
            _permissionProvider = serviceProvider.GetService<IPermissionProvider>();
            _definition = definition;
        }
        public void Authorized(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
        }

        public void HasPermissions(string userId, IIdentity targetObjectId, params string[] permissionCodes)
        {
            if (permissionCodes == null || !permissionCodes.Any()) return;

            Authorized(userId);

            if (!CheckPermissions(userId, targetObjectId, permissionCodes))
                throw new UnauthorizedAccessException();
        }

        public bool CheckPermissions(string userId, IIdentity targetObjectId, params string[] permissionCodes)
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

                if (permission.TargetAggregateType == null && targetObjectId != null)
                    return false;

            }

            var userPermissions = targetObjectId != null ? _permissionProvider.GetUserPermissions(userId, targetObjectId.Value) 
                : _permissionProvider.GetUserPermissions(userId, null);
                
            if (userPermissions == null) return false;

            var existsPermissions = userPermissions.Where(i => codes.Contains(i.PermissionCode)).Select(i=>i.PermissionCode);

            if (existsPermissions.Count() != codes.Count) return false;


            return true;
        }
    }
}