using System;
using System.Reflection;
using EventFly.Commands;
using EventFly.Permissions;
using EventFly.Security;
using FluentValidation.Results;

namespace EventFly.Infrastructure.Permissions
{
    internal class PermissionCommandValidator : ICommandValidator
    {
        private readonly ISecurityService _securityService;

        public PermissionCommandValidator(ISecurityService securityService)
        {
            _securityService = securityService;
        }
        public ValidationResult Validate(ICommand command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            var type = command.GetType();

            if (type.GetCustomAttribute<AuthorizedAttribute>() != null)
            {
                _securityService.Authorized(command.Metadata?.UserId);
            }

            //TODO: Cache
            var permissions = type.GetCustomAttribute<HasPermissionsAttribute>()?.Permissions;

            if (permissions != null)
                _securityService.HasPermissions(command.Metadata?.UserId, command.GetAggregateId(), permissions);

            return new ValidationResult();

        }

        public Int32 Priority => 0;
    }
}
