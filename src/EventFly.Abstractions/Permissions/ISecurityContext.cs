using EventFly.Core;

namespace EventFly.Permissions
{
    public interface ISecurityContext
    {
        void Authorized();
        void HasPermissions(params PermissionCode[] permissionCodes);

        void HasPermissions<TIdentity>(TIdentity targetObjectId, params PermissionCode[] permissionCodes)
            where TIdentity : IIdentity;

        System.Boolean CheckPermissions(params PermissionCode[] permissionCodes);
        System.Boolean CheckPermissions(IIdentity targetObjectId, params PermissionCode[] permissionCodes);
    }
}