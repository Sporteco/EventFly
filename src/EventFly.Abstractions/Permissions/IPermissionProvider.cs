using System.Collections.Generic;

namespace EventFly.Permissions
{

    public class PermissionInfo
    {
        public PermissionInfo(PermissionCode permissionCode, string targetObjectId)
        {
            PermissionCode = permissionCode;
            TargetObjectId = targetObjectId;
        }
        public PermissionInfo(PermissionCode permissionCode)
        {
            PermissionCode = permissionCode;
        }

        public string TargetObjectId { get; }
        public PermissionCode PermissionCode { get; }

    }
    public interface IPermissionProvider
    {
        IEnumerable<PermissionInfo> GetUserPermissions(string userId, string targetObjectId);
    }
}
