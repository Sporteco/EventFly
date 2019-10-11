using System.Collections.Generic;
using System.Linq;
using Akkatecture.Core;
using Akkatecture.Metadata;
using Newtonsoft.Json;

namespace Akkatecture.Commands
{
    public class CommandMetadata : MetadataContainer, ICommandMetadata
    {
        public CommandMetadata()
        {
        }
        public CommandMetadata(ISourceId sourceId)
        {
            SourceId = sourceId ?? Core.SourceId.New;
        }

        public CommandMetadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public CommandMetadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public CommandMetadata(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        public ISourceId SourceId
        {
            get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceId(v)); }
            set => AddValue(MetadataKeys.SourceId, value.Value);
        }

        [JsonIgnore]
        public string CorrelationId
        {
            get { return GetMetadataValue(MetadataKeys.CorrelationId); }
            set => AddValue(MetadataKeys.CorrelationId, value);
        }

        [JsonIgnore]
        public IReadOnlyCollection<string> SagaIds
        {
            get => ContainsKey(MetadataKeys.SagaIds) ? 
                (IReadOnlyCollection<string>) GetMetadataValue(MetadataKeys.SagaIds)?.Split(',') : 
                new List<string>().AsReadOnly();
            set => AddValue(MetadataKeys.SagaIds, value == null ? "" : string.Join(",",value));
        }
    }
}