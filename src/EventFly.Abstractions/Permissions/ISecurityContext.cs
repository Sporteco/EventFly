using EventFly.Core;

namespace EventFly.Permissions
{
    public interface ISecurityContext
    {
        void Authorized();
        void HasPermissions(params  PermissionCode[] permissionCodes);

        void HasPermissions<TIdentity>(TIdentity targetObjectId, params PermissionCode[] permissionCodes)
            where TIdentity : IIdentity;

        bool CheckPermissions(params  PermissionCode[] permissionCodes);
        bool CheckPermissions(IIdentity targetObjectId, params  PermissionCode[] permissionCodes);
    }
}