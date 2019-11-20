using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using EventFly.Definitions;
using EventFly.Permissions;
using EventFly.Security;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventFly.Tests.Domain
{
    public class TestPermissionsId : Identity<TestPermissionsId>
    {
        public TestPermissionsId(String value) : base(value) { }
    }

    public class TestPermissionsState : AggregateState<TestPermissionsState, TestPermissionsId> { }

    public class TestPermissionsAggregate : EventSourcedAggregateRoot<TestPermissionsAggregate, TestPermissionsId, TestPermissionsState>,
        IExecute<TestPermissionsAuthorizedAttributeCommand, TestPermissionsId>,
        IExecute<TestPermissionsAuthorizedInAggregateCommand, TestPermissionsId>,
        IExecute<TestHasPermissionAttributeSuccessCommand, TestPermissionsId>,
        IExecute<TestHasPermissionsAttributeSuccessCommand, TestPermissionsId>,
        IExecute<TestHasPermissionAttributeFailedCommand, TestPermissionsId>,
        IExecute<TestHasObjectPermissionAttributeSuccessCommand, TestPermissionsId>,
        IExecute<TestHasPermissionsAttributeFailedCommand, TestPermissionsId>
    {
        public TestPermissionsAggregate(TestPermissionsId id) : base(id) { }

        public Task<IExecutionResult> Execute(TestPermissionsAuthorizedInAggregateCommand command)
        {
            SecurityContext.Authorized();
            return Task.FromResult((IExecutionResult)new SuccessExecutionResult());
        }

        public Task<IExecutionResult> Execute(TestHasPermissionAttributeSuccessCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());

        public Task<IExecutionResult> Execute(TestHasPermissionsAttributeSuccessCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());

        public Task<IExecutionResult> Execute(TestHasPermissionAttributeFailedCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());

        public Task<IExecutionResult> Execute(TestHasPermissionsAttributeFailedCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());

        public Task<IExecutionResult> Execute(TestPermissionsAuthorizedAttributeCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());

        public Task<IExecutionResult> Execute(TestHasObjectPermissionAttributeSuccessCommand command) => Task.FromResult((IExecutionResult)new SuccessExecutionResult());
    }

    public static class TestPermissions
    {
        public const String PermissionSuccess1 = "TestPermissions:PermissionSuccess1";
        public const String PermissionSuccess2 = "TestPermissions:PermissionSuccess2";
        public const String PermissionFailed1 = "TestPermissions:PermissionFailed1";
        public const String PermissionFailed2 = "TestPermissions:PermissionFailed2";
        public const String ObjectPermission = "TestPermissions:ObjectPermission";
    }

    [HasPermissions(TestPermissions.PermissionFailed1)]
    public class TestHasPermissionAttributeFailedCommand : Command<TestPermissionsId>
    {
        public TestHasPermissionAttributeFailedCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    [HasPermissions(TestPermissions.PermissionSuccess1)]
    public class TestHasPermissionAttributeSuccessCommand : Command<TestPermissionsId>
    {
        public TestHasPermissionAttributeSuccessCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    [HasPermissions(TestPermissions.PermissionSuccess1, TestPermissions.PermissionFailed1)]
    public class TestHasPermissionsAttributeFailedCommand : Command<TestPermissionsId>
    {
        public TestHasPermissionsAttributeFailedCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    [HasPermissions(TestPermissions.PermissionSuccess1, TestPermissions.PermissionSuccess2)]
    public class TestHasPermissionsAttributeSuccessCommand : Command<TestPermissionsId>
    {
        public TestHasPermissionsAttributeSuccessCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    [HasPermissions(TestPermissions.ObjectPermission)]
    public class TestHasObjectPermissionAttributeSuccessCommand : Command<TestPermissionsId>
    {
        public TestHasObjectPermissionAttributeSuccessCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    [Authorized]
    public class TestPermissionsAuthorizedAttributeCommand : Command<TestPermissionsId>
    {
        public TestPermissionsAuthorizedAttributeCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    public class TestPermissionsAuthorizedInAggregateCommand : Command<TestPermissionsId>
    {
        public TestPermissionsAuthorizedInAggregateCommand(TestPermissionsId aggregateId, CommandMetadata metadata = null) : base(aggregateId, metadata) { }
    }

    public class PermissionProvider : IPermissionProvider
    {
        public IEnumerable<PermissionInfo> GetUserPermissions(String userId, String targetObjectId)
        {
            if (targetObjectId == "testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777")
                yield return new PermissionInfo(TestPermissions.ObjectPermission, "testpermissions-a381244e-b611-4a76-ace6-b50a4c5bb777");
            yield return new PermissionInfo(TestPermissions.PermissionSuccess1);
            yield return new PermissionInfo(TestPermissions.PermissionSuccess2);
        }
    }

    public class TestPermissionsContext : ContextDefinition
    {
        public TestPermissionsContext()
        {
            RegisterPermission(TestPermissions.PermissionSuccess1);
            RegisterPermission(TestPermissions.PermissionSuccess2);
            RegisterPermission(TestPermissions.PermissionFailed1);
            RegisterPermission(TestPermissions.PermissionFailed2);
            RegisterPermission<TestPermissionsId>(TestPermissions.ObjectPermission);

            RegisterAggregate<TestPermissionsAggregate, TestPermissionsId>();

            RegisterCommand<TestPermissionsAuthorizedAttributeCommand>();
            RegisterCommand<TestHasPermissionsAttributeSuccessCommand>();
            RegisterCommand<TestHasPermissionAttributeSuccessCommand>();
            RegisterCommand<TestHasObjectPermissionAttributeSuccessCommand>();
        }

        public override IServiceCollection DI(IServiceCollection services) => services.AddSingleton<IPermissionProvider, PermissionProvider>();
    }
}