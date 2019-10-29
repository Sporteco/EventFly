using System.Collections.Generic;

namespace EventFly.Permissions
{
    public interface IPermissionProvider
    {
        IEnumerable<IPermissionDefinition> GetUserPermissions(string userId, string targetObjectId);
    }
}
