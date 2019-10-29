using System;
using System.Collections.Generic;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Permissions;
using EventFly.Security;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.TestHelpers
{
    public class TestPermissionsId : Identity<TestPermissionsId>
    {
        public TestPermissionsId(string value) : base(value){}
    }

    public class TestPermissionsState : AggregateState<TestPermissionsAggregate,TestPermissionsId>{}

    public class TestPermissionsAggregate : EventSourcedAggregateRoot<TestPermissionsAggregate,TestPermissionsId,TestPermissionsState>
    {
        public TestPermissionsAggregate(TestPermissionsId id) : base(id){}
    }

    [Authorized]
    public class TestPermissionsAuthorizedAttributeCommand : Command<TestPermissionsId>
    {
        public TestPermissionsAuthorizedAttributeCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata){}
    }

    public class PermissionProvider : IPermissionProvider
    {
        public IEnumerable<IPermissionDefinition> GetUserPermissions(string userId, string targetObjectId)
        {
            throw new NotImplementedException();
        }
    }

    public class TestPermissionsContext : ContextDefinition
    {
        public TestPermissionsContext()
        {
            RegisterAggregate<TestPermissionsAggregate, TestPermissionsId>();
            RegisterCommand<TestPermissionsAuthorizedAttributeCommand>();

        }

        public override IServiceCollection DI(IServiceCollection services)
            => services
                .AddSingleton<IPermissionProvider, PermissionProvider>();


    }
}
