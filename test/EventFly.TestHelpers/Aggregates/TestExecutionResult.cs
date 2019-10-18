using System.Collections.Generic;
using EventFly.Commands.ExecutionResults;
using EventFly.Core;

namespace EventFly.TestHelpers.Aggregates
{
    public interface ITestExecutionResult : IExecutionResult
    {
        ISourceId SourceId { get; set; }
    }

    public static class TestExecutionResult
    {
        public static ITestExecutionResult SucceededWith(ISourceId sourceId) => 
            new SuccessTestExecutionResult(sourceId);
        public static ITestExecutionResult FailedWith(ISourceId sourceId) 
            => new FailedTestExecutionResult(sourceId, new[]{"������!!!"});
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

        public FailedTestExecutionResult(ISourceId sourceId, IEnumerable<string> errors) : base(errors)
        {
            SourceId = sourceId;
        }
    }
}