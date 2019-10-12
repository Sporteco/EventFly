using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Akkatecture.Sagas
{
    public class SagaLocatorByIdentity<TIdentity> : ISagaLocator<TIdentity> where TIdentity : IIdentity
    {
        public TIdentity LocateSaga(IDomainEvent domainEvent)
        {
            return FindSagaIdInMetadata(domainEvent.Metadata.SagaIds);
        }

        private TIdentity FindSagaIdInMetadata(IReadOnlyCollection<string> metadataSagaIds)
        {
            var sagaPrefix = GetSagaPrefix<TIdentity>();
            var sagaId = metadataSagaIds.FirstOrDefault(i => i.StartsWith(sagaPrefix + "-"));

            return sagaId == null ? CreateNewIdentity<TIdentity>() : CreateIdentity<TIdentity>(sagaId);
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
        private TIdentity CreateNewIdentity<T>() where T : TIdentity
        {
            return (TIdentity)typeof(TIdentity)
                .GetProperty(nameof(EmptyIdentity.New), BindingFlags.Public | BindingFlags.Static |  BindingFlags.FlattenHierarchy)?.GetValue(null,new object[]{});
        }
    }
    internal class EmptyIdentity : Identity<EmptyIdentity> { public EmptyIdentity(string value) : base(value){}}
}