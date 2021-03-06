﻿// The MIT License (MIT)
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

using EventFly.Metadata;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EventFly.Aggregates.Snapshot
{
    public class SnapshotMetadata : MetadataContainer, ISnapshotMetadata
    {
        public SnapshotMetadata()
        {
        }

        public SnapshotMetadata(IDictionary<System.String, System.String> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public SnapshotMetadata(IEnumerable<KeyValuePair<System.String, System.String>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public SnapshotMetadata(params KeyValuePair<System.String, System.String>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<System.String, System.String>>)keyValuePairs)
        {
        }

        [JsonIgnore]
        public System.String AggregateId
        {
            get => GetMetadataValue(SnapshotMetadataKeys.AggregateId);
            set => AddOrUpdateValue(SnapshotMetadataKeys.AggregateId, value);
        }

        [JsonIgnore]
        public System.String AggregateName
        {
            get => GetMetadataValue(SnapshotMetadataKeys.AggregateName);
            set => AddOrUpdateValue(SnapshotMetadataKeys.AggregateName, value);
        }

        [JsonIgnore]
        public System.Int64 AggregateSequenceNumber
        {
            get => GetMetadataValue(SnapshotMetadataKeys.AggregateSequenceNumber, System.Int64.Parse);
            set => AddOrUpdateValue(SnapshotMetadataKeys.AggregateSequenceNumber, value.ToString(CultureInfo.InvariantCulture));
        }

        [JsonIgnore]
        public System.String SnapshotName
        {
            get => GetMetadataValue(SnapshotMetadataKeys.SnapshotName);
            set => AddOrUpdateValue(SnapshotMetadataKeys.SnapshotName, value);
        }

        [JsonIgnore]
        public ISnapshotId SnapshotId
        {
            get => GetMetadataValue(SnapshotMetadataKeys.SnapshotId, Snapshot.SnapshotId.With);
            set => AddOrUpdateValue(SnapshotMetadataKeys.SnapshotId, value.Value);
        }

        [JsonIgnore]
        public System.Int32 SnapshotVersion
        {
            get => GetMetadataValue(SnapshotMetadataKeys.SnapshotVersion, System.Int32.Parse);
            set => AddOrUpdateValue(SnapshotMetadataKeys.SnapshotVersion, value.ToString(CultureInfo.InvariantCulture));
        }
    }
}
