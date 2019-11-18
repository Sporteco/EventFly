using EventFly.Queries;
using System.Collections.Generic;
using System.ComponentModel;

namespace Demo.Queries
{
    [Description("Тестовый запрос шоб показать проблему")]
    public class TestQuery : IQuery<IEnumerable<TestResult>>
    {

        [AllowNull]
        public NestedParams Nested { get; set; }

        public class NestedParams
        {
            [AllowNull]
            public System.String param1 { get; set; }

            public System.String param2 { get; set; }
        }
    }
    public class TestResult
    {
        public System.String Bla { get; set; }
    }
}