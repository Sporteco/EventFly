using EventFly.Aggregates.Snapshot;
using EventFly.TestHelpers.Aggregates.Snapshots;

namespace EventFly.TestHelpers.Aggregates.Sagas.TestAsync
{
    public class TestAsyncSagaSnapshot : IAggregateSnapshot<TestAsyncSaga, TestAsyncSagaId>
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public TestAggregateSnapshot.TestModel Test { get; set; }
    }
}