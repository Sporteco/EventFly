using EventFly.Aggregates.Snapshot;
using System;
using System.Collections.Generic;

namespace EventFly.TestHelpers.Aggregates.Snapshots
{
    public class TestAggregateSnapshot : IAggregateSnapshot<TestAggregate, TestAggregateId>
    {
        public List<TestModel> Tests { get; }

        public TestAggregateSnapshot(List<TestModel> tests)
        {
            Tests = tests;
        }

        public class TestModel
        {
            public Guid Id { get; }

            public TestModel(Guid id)
            {
                Id = id;
            }
        }
    }
}