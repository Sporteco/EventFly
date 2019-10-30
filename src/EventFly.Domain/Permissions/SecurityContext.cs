using EventFly.Core;

namespace EventFly.Permissions
{
    public class SecurityContext : ISecurityContext
    {
        private readonly string _userId;
        private readonly ISecurityService _securityService;

        internal SecurityContext(string userId, ISecurityService securityService)
        {
            _userId = userId;
            _securityService = securityService;
        }
        public void Authorized()
        {
            _securityService.Authorized(_userId);
        }

        public void HasPermissions(params  PermissionCode[] permissionCodes)
        {
            _securityService.HasPermissions(_userId, null, permissionCodes);
        }

        public void HasPermissions<TIdentity>(TIdentity targetObjectId, params PermissionCode[] permissionCodes) where TIdentity : IIdentity
        {
            _securityService.HasPermissions(_userId, targetObjectId, permissionCodes);
        }

        public bool CheckPermissions(params  PermissionCode[] permissionCodes)
        {
            return _securityService.CheckPermissions(_userId, null, permissionCodes);
        }

        public bool CheckPermissions(IIdentity targetObjectId, params PermissionCode[] permissionCodes)
        {
            return _securityService.CheckPermissions(_userId, targetObjectId, permissionCodes);
        }
    }
}