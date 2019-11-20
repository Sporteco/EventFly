using EventFly.Commands.ExecutionResults;
using EventFly.Core;
using System;
using System.Collections.Generic;

namespace EventFly.TestHelpers.Aggregates
{
    public interface ITestExecutionResult : IExecutionResult
    {
        ISourceId SourceId { get; set; }
    }

    public static class TestExecutionResult
    {
        public static ITestExecutionResult SucceededWith(ISourceId sourceId) => new SuccessTestExecutionResult(sourceId);
        public static ITestExecutionResult FailedWith(ISourceId sourceId) => new FailedTestExecutionResult(sourceId, new[] { "Ошибка!!!" });
    }

    public class SuccessTestExecutionResult : SuccessExecutionResult, ITestExecutionResult
    {
        public ISourceId SourceId { get; set; }

        public SuccessTestExecutionResult(ISourceId sourceId)
        {
            SourceId = sourceId;
        }
    }

    public class FailedTestExecutionResult : FailedExecutionResult, ITestExecutionResult
    {
        public ISourceId SourceId { get; set; }

        public FailedTestExecutionResult(ISourceId sourceId, IEnumerable<String> errors) : base(errors)
        {
            SourceId = sourceId;
        }
    }
}