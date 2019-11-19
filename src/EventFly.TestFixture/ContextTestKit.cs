using EventFly.Definitions;
using Xunit.Abstractions;

namespace EventFly.TestFixture
{
    public class ContextTestKit<T> : EventFlyTestKit<T>
        where T : ContextDefinition, new()
    {
        public ContextTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            BddTestHelper.Init(this);
        }
    }

    public class ContextTestKit<T1, T2> : EventFlyTestKit<T1, T2>
        where T1 : ContextDefinition, new()
        where T2 : ContextDefinition, new()
    {
        public ContextTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            BddTestHelper.Init(this);
        }
    }

    public class ContextTestKit<T1, T2, T3> : EventFlyTestKit<T1, T2, T3>
        where T1 : ContextDefinition, new()
        where T2 : ContextDefinition, new()
        where T3 : ContextDefinition, new()
    {
        public ContextTestKit(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            BddTestHelper.Init(this);
        }
    }
}