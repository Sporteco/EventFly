using EventFly.Aggregates;
using EventFly.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EventFly.Sagas
{
    public class SagaLocatorByIdentity<TIdentity> : ISagaLocator<TIdentity> where TIdentity : IIdentity
    {
        public TIdentity LocateSaga(IDomainEvent domainEvent)
        {
            return FindSagaIdInMetadata(domainEvent.Metadata.CorrelationIds, domainEvent.GetIdentity());
        }

        private TIdentity FindSagaIdInMetadata(IReadOnlyCollection<String> metadataSagaIds, IIdentity getIdentity)
        {
            var sagaPrefix = GetSagaPrefix<TIdentity>();
            var sagaId = metadataSagaIds.FirstOrDefault(i => i.StartsWith(sagaPrefix + "-"));

            return sagaId == null ? CreateNewIdentity<TIdentity>(getIdentity) : CreateIdentity<TIdentity>(sagaId);
        }

        //TODO: remove reflection
        private String GetSagaPrefix<T>() where T : TIdentity
        {
            return (String)typeof(TIdentity)
                .GetProperty(nameof(EmptyIdentity.IdentityPrefix), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)?.GetValue(null, new Object[] { });
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
        private TIdentity CreateNewIdentity<T>(IIdentity getIdentity) where T : TIdentity
        {
            var guid = Guid.Parse(getIdentity.Value.Substring(getIdentity.Value.IndexOf('-') + 1));
            return (TIdentity)typeof(TIdentity)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .FirstOrDefault(i => i.Name == nameof(EmptyIdentity.With) && i.GetParameters().Any(p => p.ParameterType.Name.Contains("Guid")))
                ?.Invoke(null, new Object[] { guid });
        }
    }
    internal class EmptyIdentity : Identity<EmptyIdentity> { public EmptyIdentity(String value) : base(value) { } }
}