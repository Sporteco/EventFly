using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Infrastructure.Definitions;
using EventFly.TestFixture.Internals;
using EventFly.TestFixture.Internals.AggregateFixture;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit.Abstractions;

namespace EventFly.TestFixture
{
    public class AggregateTestKit<T> : EventFlyTestKit<T>
        where T : ContextDefinition, new()
    {
        public AggregateTestKit(
            ITestOutputHelper testOutputHelper, 
            Func<IServiceCollection, IServiceCollection> myDependencies = null
        ) : base(testOutputHelper, myDependencies) { }

        public IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregate, TIdentity>(TIdentity aggregateId)
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(this).For(aggregateId);
        }
    }

    public class AggregateTestKit<T1, T2> : EventFlyTestKit<T1, T2>
        where T1 : ContextDefinition, new()
        where T2 : ContextDefinition, new()
    {
        public AggregateTestKit(
            ITestOutputHelper testOutputHelper, 
            Func<IServiceCollection, IServiceCollection> myDependencies1 = null,
            Func<IServiceCollection, IServiceCollection> myDependencies2 = null) : base(testOutputHelper, myDependencies1, myDependencies2) { }

        public IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregate, TIdentity>(TIdentity aggregateId)
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(this).For(aggregateId);
        }
    }

    public class AggregateTestKit<T1, T2, T3> : EventFlyTestKit<T1, T2, T3>
        where T1 : ContextDefinition, new()
        where T2 : ContextDefinition, new()
        where T3 : ContextDefinition, new()
    {
        public AggregateTestKit(
            ITestOutputHelper testOutputHelper, 
            Func<IServiceCollection, IServiceCollection> myDependencies1 = null,
            Func<IServiceCollection, IServiceCollection> myDependencies2 = null,
            Func<IServiceCollection, IServiceCollection> myDependencies3 = null) : base(testOutputHelper, myDependencies1, myDependencies2, myDependencies3) { }

        public IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregate, TIdentity>(TIdentity aggregateId)
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(this).For(aggregateId);
        }
    }
}