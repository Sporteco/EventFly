using System.Collections.Generic;

namespace EventFly.Permissions
{

    public class PermissionInfo
    {
        public PermissionInfo(PermissionCode permissionCode, System.String targetObjectId)
        {
            PermissionCode = permissionCode;
            TargetObjectId = targetObjectId;
        }
        public PermissionInfo(PermissionCode permissionCode)
        {
            PermissionCode = permissionCode;
        }

        public System.String TargetObjectId { get; }
        public PermissionCode PermissionCode { get; }

    }
    public interface IPermissionProvider
    {
        IEnumerable<PermissionInfo> GetUserPermissions(System.String userId, System.String targetObjectId);
    }
}
