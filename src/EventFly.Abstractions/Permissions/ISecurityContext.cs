using EventFly.Core;

namespace EventFly.Permissions
{
    public interface ISecurityContext
    {
        void Authorized();
        void HasPermissions(params  string[] permissionCodes);

        void HasPermissions<TIdentity>(TIdentity targetObjectId, params string[] permissionCodes)
            where TIdentity : IIdentity;

        bool CheckPermissions(params  string[] permissionCodes);
        bool CheckPermissions(IIdentity targetObjectId, params  string[] permissionCodes);
    }
}