﻿// The MIT License (MIT)
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

using EventFly.Core;
using EventFly.Extensions;
using EventFly.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventFly.Aggregates
{
    public class EventMetadata : MetadataContainer, IEventMetadata
    {
        public static IEventMetadata Empty { get; } = new EventMetadata();

        public static IEventMetadata With(IEnumerable<KeyValuePair<String, String>> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static IEventMetadata With(params KeyValuePair<String, String>[] keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public static IEventMetadata With(IDictionary<String, String> keyValuePairs)
        {
            return new EventMetadata(keyValuePairs);
        }

        public ISourceId SourceId
        {
            get { return GetMetadataValue(MetadataKeys.SourceId, v => new SourceId(v)); }
            set => AddOrUpdateValue(MetadataKeys.SourceId, value.Value);
        }

        [JsonIgnore]
        public String EventName
        {
            get => GetMetadataValue(MetadataKeys.EventName);
            set => AddOrUpdateValue(MetadataKeys.EventName, value);
        }

        [JsonIgnore]
        public Int32 EventVersion
        {
            get => GetMetadataValue(MetadataKeys.EventVersion, Int32.Parse);
            set => AddOrUpdateValue(MetadataKeys.EventVersion, value.ToString());
        }

        [JsonIgnore]
        public DateTimeOffset Timestamp
        {
            get => GetMetadataValue(MetadataKeys.Timestamp, DateTimeOffset.Parse);
            set => AddOrUpdateValue(MetadataKeys.Timestamp, value.ToString("O"));
        }

        [JsonIgnore]
        public Int64 TimestampEpoch
        {
            get
            {
#pragma warning disable IDE0018 // Inline variable declaration
                String timestampEpoch;
#pragma warning restore IDE0018 // Inline variable declaration
                return TryGetValue(MetadataKeys.TimestampEpoch, out timestampEpoch)
                    ? Int64.Parse(timestampEpoch)
                    : Timestamp.ToUnixTime();
            }
        }

        [JsonIgnore]
        public Int64 AggregateSequenceNumber
        {
            get => GetMetadataValue(MetadataKeys.AggregateSequenceNumber, Int32.Parse);
            set => AddOrUpdateValue(MetadataKeys.AggregateSequenceNumber, value.ToString());
        }

        [JsonIgnore]
        public String AggregateId
        {
            get => GetMetadataValue(MetadataKeys.AggregateId);
            set => AddOrUpdateValue(MetadataKeys.AggregateId, value);
        }

        [JsonIgnore]
        public String CorrelationId
        {
            get => GetMetadataValue(MetadataKeys.CorrelationId);
            set => AddOrUpdateValue(MetadataKeys.CorrelationId, value);
        }

        [JsonIgnore]
        public IReadOnlyCollection<String> CorrelationIds
        {
            get => ContainsKey(MetadataKeys.CorrelationIds) ?
                (IReadOnlyCollection<String>)GetMetadataValue(MetadataKeys.CorrelationIds)?.Split(',') :
                new List<String>().AsReadOnly();
            set => AddOrUpdateValue(MetadataKeys.CorrelationIds, value == null ? "" : String.Join(",", value));
        }


        [JsonIgnore]
        public String CausationId
        {
            get => GetMetadataValue(MetadataKeys.CausationId);
            set => AddOrUpdateValue(MetadataKeys.CausationId, value);
        }

        [JsonIgnore]
        public IEventId EventId
        {
            get => GetMetadataValue(MetadataKeys.EventId, Aggregates.EventId.With);
            set => AddOrUpdateValue(MetadataKeys.EventId, value.Value);
        }

        [JsonIgnore]
        public String AggregateName
        {
            get => GetMetadataValue(MetadataKeys.AggregateName);
            set => AddOrUpdateValue(MetadataKeys.AggregateName, value);
        }

        public EventMetadata()
        {
            // Empty
        }

        public EventMetadata(IDictionary<String, String> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public EventMetadata(IEnumerable<KeyValuePair<String, String>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public EventMetadata(params KeyValuePair<String, String>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<String, String>>)keyValuePairs)
        {
        }

        public IEventMetadata CloneWith(params KeyValuePair<String, String>[] keyValuePairs)
        {
            return CloneWith((IEnumerable<KeyValuePair<String, String>>)keyValuePairs);
        }

        public IEventMetadata CloneWith(IEnumerable<KeyValuePair<String, String>> keyValuePairs)
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
