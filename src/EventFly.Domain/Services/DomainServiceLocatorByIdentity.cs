using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventFly.Core;

namespace EventFly.Domain.Services
{
    public class DomainServiceLocatorByIdentity<TIdentity> : IDomainServiceLocator<TIdentity> where TIdentity : IIdentity
    {
        public TIdentity LocateService(IDomainEvent domainEvent)
        {
            return FindSagaIdInMetadata(domainEvent.Metadata.CorrellationIds, domainEvent.GetIdentity());
        }

        private TIdentity FindSagaIdInMetadata(IReadOnlyCollection<string> metadataSagaIds, IIdentity getIdentity)
        {
            var sagaPrefix = GetSagaPrefix<TIdentity>();
            var sagaId = metadataSagaIds.FirstOrDefault(i => i.StartsWith(sagaPrefix + "-"));

            return sagaId == null ? CreateNewIdentity<TIdentity>(getIdentity) : CreateIdentity<TIdentity>(sagaId);
        }

        //TODO: remove reflection
        private string GetSagaPrefix<T>() where T : TIdentity
        {
            return (string)typeof(TIdentity)
                .GetProperty(nameof(EmptyIdentity.IdentityPrefix), BindingFlags.Public | BindingFlags.Static |  BindingFlags.FlattenHierarchy)?.GetValue(null,new object[]{});
        }

        //TODO: remove reflection
        private TIdentity CreateIdentity<T>(string id) where T : TIdentity
        {
            return (TIdentity)typeof(TIdentity)
                .GetMethods(BindingFlags.Public | BindingFlags.Static |  BindingFlags.FlattenHierarchy)
                .FirstOrDefault(i => i.Name == nameof(EmptyIdentity.With) && i.GetParameters().Any(p => p.ParameterType.Name.Contains("String")))
                ?.Invoke(null,new object[]{id});
        }
        //TODO: remove reflection
        private TIdentity CreateNewIdentity<T>(IIdentity getIdentity) where T : TIdentity
        {
            var guid = Guid.Parse(getIdentity.Value.Substring(getIdentity.Value.IndexOf('-')+1));
            return (TIdentity)typeof(TIdentity)
                .GetMethods(BindingFlags.Public | BindingFlags.Static |  BindingFlags.FlattenHierarchy)
                .FirstOrDefault(i => i.Name == nameof(EmptyIdentity.With) && i.GetParameters().Any(p => p.ParameterType.Name.Contains("Guid")))
                ?.Invoke(null,new object[]{guid});
        }
    }
}