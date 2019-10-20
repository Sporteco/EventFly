using EventFly.Definitions;
using System.Collections.Generic;

namespace EventFly.DependencyInjection
{
    public sealed class DomainsBuilder
    {
        public sealed class DomainBuilder<TDomain>
            where TDomain : IDomainDefinition
        {
            private readonly TDomain _domain;
            public DomainBuilder(TDomain domain)
            {
                _domain = domain;
            }

            public DomainBuilder<TDomain> WithDependencies<TDomainDependencies>()
                where TDomainDependencies : IDomainDependencies<IDomainDefinition>, new()
            {
                var deps = new TDomainDependencies();

                if (_domain is DomainDefinition assignableDomain)
                    assignableDomain.DomainDependencies = deps;

                return this;
            }
        }

        public IList<IDomainDefinition> Domains { get; } = new List<IDomainDefinition>();

        public DomainBuilder<TDomain> RegisterDomainDefinitions<TDomain>()
            where TDomain : IDomainDefinition, new()
        {
            var domain = new TDomain();
            Domains.Add(domain);
            return new DomainBuilder<TDomain>(domain);
        }
    }
}
