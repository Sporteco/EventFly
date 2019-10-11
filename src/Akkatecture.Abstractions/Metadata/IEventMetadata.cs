using System.Collections.Generic;
using Akkatecture.Core;

namespace Akkatecture.Metadata
{
    public interface ICommonMetadata : IMetadataContainer
    {
        ISourceId SourceId { get; }
        string CorrelationId { get; }
        IReadOnlyCollection<string> SagaIds { get; }
    }
}
