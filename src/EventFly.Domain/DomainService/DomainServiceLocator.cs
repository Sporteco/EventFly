using EventFly.Aggregates;
using EventFly.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventFly.Domain
{
    public class DomainServiceLocator<TIdentity> : IDomainServiceLocator<TIdentity>
        where TIdentity : IIdentity
    {
        public TIdentity LocateDomainService(IDomainEvent domainEvent)
        {
            return FindDomainServiceIdInMetadata(domainEvent.Metadata.CorrellationIds, domainEvent.GetIdentity());
        }

        private TIdentity FindDomainServiceIdInMetadata(IReadOnlyCollection<String> metadataDomainServiceIds, IIdentity id)
        {
            var domainServicePrefix = GetDomainServicePrefix<TIdentity>();
            var domainServiceId = metadataDomainServiceIds.FirstOrDefault(i => i.StartsWith(domainServicePrefix + "-"));

            return domainServiceId == null ? CreateNewIdentity<TIdentity>(id) : CreateIdentity<TIdentity>(domainServiceId);
        }

        //TODO: remove reflection
        private String GetDomainServicePrefix<T>() where T : TIdentity
        {
            return (String)typeof(TIdentity)
                .GetProperty(nameof(EmptyIdentity.IdentityPrefix), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?
                .GetValue(null, Array.Empty<Object>());
        }

        //TODO: remove reflection
        private TIdentity CreateIdentity<T>(String id) where T : TIdentity
        {
            return (TIdentity)typeof(TIdentity)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .FirstOrDefault(i => i.Name == nameof(EmptyIdentity.With) && i.GetParameters().Any(p => p.ParameterType.Name.Contains("String")))
                ?.Invoke(null, new Object[] { id });
        }

        //TODO: remove reflection
        private TIdentity CreateNewIdentity<T>(IIdentity id) where T : TIdentity
        {
            var guid = Guid.Parse(id.Value.Substring(id.Value.IndexOf('-') + 1));
            return (TIdentity)typeof(TIdentity)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .FirstOrDefault(i => i.Name == nameof(EmptyIdentity.With) && i.GetParameters().Any(p => p.ParameterType.Name.Contains("Guid")))
                ?.Invoke(null, new Object[] { guid });
        }
    }
}