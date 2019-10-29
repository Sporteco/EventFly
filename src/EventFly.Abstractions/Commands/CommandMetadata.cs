using System.Collections.Generic;
using System.Linq;
using EventFly.Core;
using EventFly.Metadata;
using Newtonsoft.Json;

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
            private set => AddOrUpdateValue(MetadataKeys.SourceId, value.Value);
        }

        [JsonIgnore]
        public string CorrelationId
        {
            get => GetMetadataValue(MetadataKeys.CorrelationId);
            set => AddOrUpdateValue(MetadataKeys.CorrelationId, value);
        }

        [JsonIgnore]
        public IReadOnlyCollection<string> CorrelationIds
        {
            get => ContainsKey(MetadataKeys.CorrelationIds) ? 
                (IReadOnlyCollection<string>) GetMetadataValue(MetadataKeys.CorrelationIds)?.Split(',') : 
                new List<string>().AsReadOnly();
            set => AddOrUpdateValue(MetadataKeys.CorrelationIds, value == null ? "" : string.Join(",",value));
        }
    }
}