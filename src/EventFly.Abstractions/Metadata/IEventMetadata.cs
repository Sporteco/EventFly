using System.Collections.Generic;
using EventFly.Core;

namespace EventFly.Metadata
{
    public interface ICommonMetadata : IMetadataContainer
    {
        ISourceId SourceId { get; }
        string CorrelationId { get; }
        IReadOnlyCollection<string> SagaIds { get; }
    }
}
