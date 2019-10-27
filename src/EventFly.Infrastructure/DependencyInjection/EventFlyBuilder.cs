using System;
using System.Linq;
using System.Reflection;
using EventFly.Definitions;
using EventFly.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EventFly.DependencyInjection
{
    public sealed class EventFlyBuilder
    {
        public IServiceCollection Services { get; }
        private readonly ApplicationDefinition _applicationDefinition;
        public IApplicationDefinition ApplicationDefinition => _applicationDefinition;

        public EventFlyBuilder(IServiceCollection services)
        {
            Services = services;
            _applicationDefinition = new ApplicationDefinition();
            Services
                .AddSingleton<IApplicationDefinition>(_applicationDefinition)
                .AddSingleton<IDefinitionToManagerRegistry, DefinitionToManagerRegistry>();
        }

        public EventFlyBuilder WithContext<TContext>()
            where TContext : ContextDefinition, new()
        {
            var context = new TContext();
            context.DI(Services);


            _applicationDefinition.RegisterContext(context);

            RegisterValidators(Services);
            return this;
        }

        private void RegisterValidators(IServiceCollection services)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                RegisterValidatorsInAssembly(assembly, services);
            }
        }

        private void RegisterValidatorsInAssembly(Assembly assembly, IServiceCollection services)
        {
            var validatorTypes = assembly.GetTypes().SelectMany(i => i.GetCustomAttributes<ValidatorAttribute>()
                .Select(j => new {Type=i, j.ValidatorType})).Distinct();
            foreach (var item in validatorTypes)
            {
                services.AddSingleton(typeof(IValidator<>).MakeGenericType(item.Type), item.ValidatorType);
            }

        }
    }
}
