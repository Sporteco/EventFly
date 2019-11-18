using EventFly.Core;
using EventFly.Metadata;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Commands
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

        public CommandMetadata(IDictionary<System.String, System.String> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public CommandMetadata(IEnumerable<KeyValuePair<System.String, System.String>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public CommandMetadata(params KeyValuePair<System.String, System.String>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<System.String, System.String>>)keyValuePairs)
        {
        }

        public ISourceId SourceId
        {
            get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceId(v)); }
            private set => AddOrUpdateValue(MetadataKeys.SourceId, value.Value);
        }

        [JsonIgnore]
        public System.String CorrelationId
        {
            get => GetMetadataValue(MetadataKeys.CorrelationId);
            set => AddOrUpdateValue(MetadataKeys.CorrelationId, value);
        }

        [JsonIgnore]
        public IReadOnlyCollection<System.String> CorrelationIds
        {
            get => ContainsKey(MetadataKeys.CorrelationIds) ?
                (IReadOnlyCollection<System.String>)GetMetadataValue(MetadataKeys.CorrelationIds)?.Split(',') :
                new List<System.String>().AsReadOnly();
            set => AddOrUpdateValue(MetadataKeys.CorrelationIds, value == null ? "" : System.String.Join(",", value));
        }
    }
}