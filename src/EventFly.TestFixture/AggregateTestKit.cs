using Akka.Actor;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Infrastructure.Definitions;
using EventFly.TestFixture.Internals;
using EventFly.TestFixture.Internals.AggregateFixture;
using Xunit.Abstractions;

namespace EventFly.TestFixture
{
    public class AggregateTestKit<T> : EventFlyTestKit<T>
        where T : ContextDefinition, new()
    {
        public AggregateTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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
        public AggregateTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

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
        public AggregateTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        public IFixtureArranger<TAggregate, TIdentity> FixtureFor<TAggregate, TIdentity>(TIdentity aggregateId)
            where TAggregate : ActorBase, IAggregateRoot<TIdentity>
            where TIdentity : IIdentity
        {
            return new AggregateFixture<TAggregate, TIdentity>(this).For(aggregateId);
        }
    }
}