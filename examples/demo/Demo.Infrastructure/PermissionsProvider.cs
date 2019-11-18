using EventFly.Permissions;
using System.Collections.Generic;

namespace Demo.Infrastructure
{
    public class PermissionProvider : IPermissionProvider
    {

        public IEnumerable<PermissionInfo> GetUserPermissions(System.String userId, System.String targetObjectId)
        {
            yield return new PermissionInfo(DemoContext.CreateUser);
        }
    }
}