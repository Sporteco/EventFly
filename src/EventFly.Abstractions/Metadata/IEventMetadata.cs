using EventFly.Core;
using System.Collections.Generic;

namespace EventFly.Metadata
{
    public interface ICommonMetadata : IMetadataContainer
    {
        ISourceId SourceId { get; }
        System.String CorrelationId { get; }
        IReadOnlyCollection<System.String> CorrelationIds { get; }
    }
}
