using System.Collections.Generic;
using EventFly.Permissions;

namespace Demo.Infrastructure
{
    public class PermissionProvider : IPermissionProvider
    {

        public IEnumerable<IPermissionDefinition> GetUserPermissions(string userId, string targetObjectId)
        {
            yield return new PermissionDefinition(DemoContext.CreateUser);
        }
    }
}