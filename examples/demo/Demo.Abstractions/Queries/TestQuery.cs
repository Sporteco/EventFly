using System.Collections.Generic;
using System.ComponentModel;
using EventFly.Queries;

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
            public string param1 { get; set; }
           
            public string param2 { get; set; }
        }
    }
    public class TestResult
    {
        public string Bla { get; set; }
    }
}