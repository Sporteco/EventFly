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

namespace EventFly.Metadata
{
    public static class MetadataKeys
    {
        public const System.String UserId = "user_id";
        public const System.String EventId = "event_id";
        public const System.String BatchId = "batch_id";
        public const System.String EventName = "event_name";
        public const System.String EventVersion = "event_version";
        public const System.String Timestamp = "timestamp";
        public const System.String TimestampEpoch = "timestamp_epoch";
        public const System.String AggregateSequenceNumber = "aggregate_sequence_number";
        public const System.String AggregateName = "aggregate_name";
        public const System.String AggregateId = "aggregate_id";
        public const System.String SourceId = "source_id";
        public const System.String CorrelationId = "correlation_id";
        public const System.String CorrelationIds = "correlation_ids";
        public const System.String CausationId = "causation_id";
    }
}
