﻿// The MIT License (MIT)
//
// Copyright (c) 2015-2019 Rasmus Mikkelsen
// Copyright (c) 2015-2019 eBay Software Foundation
// Modified from original source https://github.com/eventflow/EventFlow
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/Akkatecture 
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
using System.Threading;
using Akkatecture.Commands.ExecutionResults;
using Akkatecture.Core;

namespace Akkatecture.Commands
{
    public abstract class DistinctCommand<TIdentity> : ICommand<TIdentity, IExecutionResult>
        where TIdentity : IIdentity
    {
        private readonly Lazy<ISourceId> _lazySourceId;

        public ISourceId SourceId => _lazySourceId.Value;
        public TIdentity AggregateId { get; }

        protected DistinctCommand(
            TIdentity aggregateId)
        {
            if (aggregateId == null) 
                throw new ArgumentNullException(nameof(aggregateId));

            _lazySourceId = new Lazy<ISourceId>(CalculateSourceId, LazyThreadSafetyMode.PublicationOnly);

            AggregateId = aggregateId;
        }

        private CommandId CalculateSourceId()
        {
            var bytes = GetSourceIdComponents().SelectMany(b => b).ToArray();
            return CommandId.NewDeterministic(
                GuidFactories.Deterministic.Namespaces.Commands,
                bytes);
        }

        protected abstract IEnumerable<byte[]> GetSourceIdComponents();

        
        public ISourceId GetSourceId()
        {
            return SourceId;
        }

        public IIdentity GetAggregateId() => AggregateId;
    }
}