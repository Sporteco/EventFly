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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventFly.Metadata;
using Newtonsoft.Json;

namespace EventFly.Aggregates.Snapshot
{
    public class SnapshotMetadata : MetadataContainer, ISnapshotMetadata
    {
        public SnapshotMetadata()
        {
        }

        public SnapshotMetadata(IDictionary<string, string> keyValuePairs)
            : base(keyValuePairs)
        {
        }

        public SnapshotMetadata(IEnumerable<KeyValuePair<string, string>> keyValuePairs)
            : base(keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value))
        {
        }

        public SnapshotMetadata(params KeyValuePair<string, string>[] keyValuePairs)
            : this((IEnumerable<KeyValuePair<string, string>>)keyValuePairs)
        {
        }

        [JsonIgnore]
        public string AggregateId
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.AggregateId); }
            set { AddValue(SnapshotMetadataKeys.AggregateId, value); }
        }

        [JsonIgnore]
        public string AggregateName
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.AggregateName); }
            set { AddValue(SnapshotMetadataKeys.AggregateName, value); }
        }

        [JsonIgnore]
        public long AggregateSequenceNumber
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.AggregateSequenceNumber, long.Parse); }
            set { AddValue(SnapshotMetadataKeys.AggregateSequenceNumber, value.ToString(CultureInfo.InvariantCulture)); }
        }

        [JsonIgnore]
        public string SnapshotName
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.SnapshotName); }
            set { AddValue(SnapshotMetadataKeys.SnapshotName, value); }
        }
        
        [JsonIgnore]
        public ISnapshotId SnapshotId
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.SnapshotId, Snapshot.SnapshotId.With); }
            set { AddValue(SnapshotMetadataKeys.SnapshotId, value.Value); }
        }

        [JsonIgnore]
        public int SnapshotVersion
        {
            get { return GetMetadataValue(SnapshotMetadataKeys.SnapshotVersion, int.Parse); }
            set { AddValue(SnapshotMetadataKeys.SnapshotVersion, value.ToString(CultureInfo.InvariantCulture)); }
        }
    }
}