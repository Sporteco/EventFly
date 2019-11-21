using EventFly.Definitions;
using EventFly.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace EventFly.DependencyInjection
{
    public sealed class EventFlyBuilder
    {
        public IServiceCollection Services { get; }
        public IApplicationDefinition ApplicationDefinition { get; private set; }

        public EventFlyBuilder(IServiceCollection services)
        {
            Services = services;
            ApplicationDefinition = new ApplicationDefinition();
            Services
                .AddSingleton(ApplicationDefinition)
                .AddSingleton<IDefinitionToManagerRegistry, DefinitionToManagerRegistry>();
        }

        public EventFlyBuilder WithContext<TContext>()
            where TContext : ContextDefinition, new()
        {
            var context = new TContext();
            context.DI(Services);
            ((ApplicationDefinition)ApplicationDefinition).RegisterContext(context);
            RegisterValidators(Services);
            return this;
        }

        private static void RegisterValidators(IServiceCollection services)
        {
            //bug workaround, see: https://developercommunity.visualstudio.com/content/problem/738856/could-not-load-file-or-assembly-microsoftintellitr.html
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.FullName.StartsWith("Microsoft."));
            foreach (var assembly in assemblies) RegisterValidatorsInAssembly(assembly, services);
        }

        private static void RegisterValidatorsInAssembly(Assembly assembly, IServiceCollection services)
        {
            try
            {
                var validatorTypes = assembly
                    .GetTypes()
                    .SelectMany(i => i.GetCustomAttributes<ValidatorAttribute>()
                    .Select(j => new { Type = i, j.ValidatorType }))
                    .Distinct();
                foreach (var item in validatorTypes)
                {
                    services.TryAddSingleton(typeof(IValidator<>).MakeGenericType(item.Type), item.ValidatorType);
                }
            }
            catch
            {
                //ignore
            }
        }
    }
}