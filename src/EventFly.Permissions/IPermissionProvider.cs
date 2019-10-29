using System.Collections.Generic;
using EventFly.Definitions;

namespace EventFly.Permissions
{
    public interface IPermissionProvider
    {
        IEnumerable<IPermissionDefinition> GetUserPermissions(string userId);
    }
}
