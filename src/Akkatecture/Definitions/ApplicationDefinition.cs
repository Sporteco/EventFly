using EventFly.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Akkatecture.Meta;

namespace EventFly.Application.UseCases
{
    /// <summary>
    /// Define actions served by server.
    /// </summary>
    public class ApplicationMeta
    {
        public ApplicationMeta(IEnumerable<Type> actionTypes, IEnumerable<DomainMeta> underlyingDomains)
        {
            Domains = underlyingDomains.ToImmutableHashSet();

            var cmds = new List<CommandDefinition>();
            var prjs = new List<ProjectionDefinition>();
            var sagas = new List<SagaMeta>();
            var aggs = Domains.SelectMany(x => x.Aggregates);
            foreach (var actionType in actionTypes)
            {
                var t = actionType.GetTypeInfo();
                if (t.IsSubclassOf(typeof(Command)))
                {
                    var aggMeta = aggs.Single(x => x.Type == t.BaseType.GenericTypeArguments[0]);
                    cmds.Add(new CommandDefinition(actionType, aggMeta));
                }
                else if (t.IsSubclassOf(typeof(Projection)))
                {
                    var aggMeta = aggs.Single(x => x.Type == t.GetCustomAttribute<AggregateAttribute>().Type);
                    prjs.Add(new ProjectionDefinition(actionType, aggMeta));
                }
                else if (t.IsSubclassOf(typeof(Saga)))
                {
                    sagas.Add(new SagaMeta(actionType));
                }
            }
            PublicCommands = cmds.Where(x => !x.IsInternal).ToImmutableHashSet();
            _publicAndInternalCommands = cmds.ToImmutableHashSet();
            Projections = prjs.ToImmutableHashSet();
            Sagas = sagas.ToImmutableHashSet();
        }

        public IReadOnlyCollection<DomainMeta> Domains { get; }
        public IReadOnlyCollection<CommandDefinition> PublicCommands { get; }
        public IReadOnlyCollection<ProjectionDefinition> Projections { get; }
        public IReadOnlyCollection<SagaMeta> Sagas { get; }

        public virtual IEnumerable<CommandDefinition> GetInitializeCommands()
        {
            var singletonAggregateMetas = Domains.SelectMany(x => x.Aggregates).Where(x => x.SingletonId != null);
            foreach (var aggregateMeta in singletonAggregateMetas)
            {
                var createCommandMeta = _publicAndInternalCommands.SingleOrDefault(x => x.Aggregate == aggregateMeta && x.IsInternal && x.Name.Equals("Create"));
                if (createCommandMeta == null)
                {
                    throw new Exception($"Singleton aggregate {aggregateMeta} must have internal creation behavior!");
                }
                yield return createCommandMeta;
            }
        }

        public AggregateMeta GetAggregate(Type aggregateOrProjectionType)
        {
            var t = aggregateOrProjectionType.GetTypeInfo();

            if (t.IsSubclassOf(typeof(Aggregate)))
                return Domains.SelectMany(x => x.Aggregates).Single(x => x.Type == aggregateOrProjectionType);

            if (t.IsSubclassOf(typeof(Projection)))
                return GetProjection(aggregateOrProjectionType).Aggregate;

            throw new Exception("Aggregate not found!");
        }

        public AggregateMeta GetAggregateByProjection(Type projectionType) => GetProjection(projectionType).Aggregate;

        public ProjectionDefinition GetProjection(Type projectionOrQueryType) => Projections.Single(x => x.Type == projectionOrQueryType || x.Query.Type == projectionOrQueryType);

        public CommandDefinition GetPublicCommand(Command command) => PublicCommands.Single(x => x.Type == command.GetType());

        public CommandDefinition GetPublicOrInternalCommand(Command command) => _publicAndInternalCommands.Single(x => x.Type == command.GetType());

        private readonly ImmutableHashSet<CommandDefinition> _publicAndInternalCommands;
    }
}