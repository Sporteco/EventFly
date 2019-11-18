using EventFly.Core;

namespace EventFly.Permissions
{
    public interface ISecurityService
    {
        void Authorized(System.String userId);
        void HasPermissions(System.String userId, IIdentity targetObjectId, params PermissionCode[] permissionCodes);
        System.Boolean CheckPermissions(System.String userId, IIdentity targetObjectId, params PermissionCode[] permissionCodes);
    }
}