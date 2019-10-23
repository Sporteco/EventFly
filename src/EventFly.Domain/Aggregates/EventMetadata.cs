// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/EventFly 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Metadata;
using Newtonsoft.Json;

namespace EventFly.Domain.Aggregates
{
    public class EventMetadata : MetadataContainer, IEventMetadata
    {
        public static IEventMetadata Empty { get; } = new EventMetadata();

        public static IEventMetadata With(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static IEventMetadata With(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static IEventMetadata With(IDictionary<string, string> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public ISourceId SourceId
        {
            get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceId(v)); }
            set => AddValue(MetadataKeys.SourceId, value.Value);
        }

        [JsonIgnore]
        public string EventName
        {
            get => GetMetadataValue(MetadataKeys.EventName);
            set => AddValue(MetadataKeys.EventName, value);
        }

        [JsonIgnore]
        public int EventVersion
        {
            get => GetMetadataValue(MetadataKeys.EventVersion, int.Parse);
            set => AddValue(MetadataKeys.EventVersion, value.ToString());
        }

        [JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get => GetMetadataValue(MetadataKeys.Timestamp, DateTimeOffset.Parse);
            set => AddValue(MetadataKeys.Timestamp, value.ToString("O"));
        }

        [JsonIgnore]
        public long TimestampEpoch
        {
            get
            {
#pragma warning disable IDE0018 // Inline variable declaration
                string timestampEpoch;
#pragma warning restore IDE0018 // Inline variable declaration
                return TryGetValue(MetadataKeys.TimestampEpoch, out timestampEpoch)
                    ? long.Parse(timestampEpoch)
                    : Timestamp.ToUnixTime();
            }
        }

        [JsonIgnore]
        public long AggregateSequenceNumber
        {
            get => GetMetadataValue(MetadataKeys.AggregateSequenceNumber, int.Parse);
            set => AddValue(MetadataKeys.AggregateSequenceNumber, value.ToString());
        }

        [JsonIgnore]
        public string AggregateId
        {
            get => GetMetadataValue(MetadataKeys.AggregateId);
            set => AddValue(MetadataKeys.AggregateId, value);
        }
        
        [JsonIgnore]
        public string CorrelationId
        {
            get => GetMetadataValue(MetadataKeys.CorrelationId);
            set => AddValue(MetadataKeys.CorrelationId, value);
        }

        [JsonIgnore]
        public IReadOnlyCollection<string> CorrellationIds
        {
            get => ContainsKey(MetadataKeys.SagaIds) ? 
                (IReadOnlyCollection<string>) GetMetadataValue(MetadataKeys.SagaIds)?.Split(',') : 
                new List<string>().AsReadOnly();
            set => AddValue(MetadataKeys.SagaIds, value == null ? "" : string.Join(",",value));
        }


        [JsonIgnore]
        public string CausationId
        {
            get => GetMetadataValue(MetadataKeys.CausationId);
            set => AddValue(MetadataKeys.CausationId, value);
        }

        [JsonIgnore]
        public IEventId EventId
        {
            get => GetMetadataValue(MetadataKeys.EventId, EventFly.Aggregates.EventId.With);
            set => AddValue(MetadataKeys.EventId, value.Value);
        }

        [JsonIgnore]
        public string AggregateName
        {
            get => GetMetadataValue(MetadataKeys.AggregateName);
            set => AddValue(MetadataKeys.AggregateName, value);
        }

        public EventMetadata()
        {
            // Empty
        }

        public EventMetadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public EventMetadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public EventMetadata(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        public IEventMetadata CloneWith(params KeyValuePair<string, string>[] keyValuePairs)
        {
            return CloneWith((IEnumerable<KeyValuePair<string, string>>)keyValuePairs);
        }

        public IEventMetadata CloneWith(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var metadata = new EventMetadata(this);
            foreach (var kv in keyValuePairs)
            {
                if (metadata.ContainsKey(kv.Key))
                {
                    throw new ArgumentException($"Key '{kv.Key}' is already present!");
                }
                metadata[kv.Key] = kv.Value;
            }
            return metadata;
        }


    }
}
