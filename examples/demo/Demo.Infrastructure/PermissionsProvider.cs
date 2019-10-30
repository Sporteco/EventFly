using System.Collections.Generic;
using EventFly.Permissions;

namespace Demo.Infrastructure
{
    public class PermissionProvider : IPermissionProvider
    {

        public IEnumerable<PermissionInfo> GetUserPermissions(string userId, string targetObjectId)
        {
            yield return new PermissionInfo(DemoContext.CreateUser);
        }
    }
}