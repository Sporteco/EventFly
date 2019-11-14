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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using EventFly.Aggregates;
using EventFly.Core;
using EventFly.Exceptions;
using EventFly.Extensions;
using EventFly.TestHelpers;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.UnitTests.Extensions
{
    [Category(Categories.Unit)]
    public class TypeExtensionTests
    {
        [Theory]
        [InlineData(typeof(EventFly.Aggregates.AggregateRoot<,,>), "EventFly.AggregateRoot<,,>")]
        [InlineData(typeof(string), "System.String")]
        [InlineData(typeof(int), "System.Int32")]
        [InlineData(typeof(IEnumerable<>), "System.IEnumerable<>")]
        [InlineData(typeof(KeyValuePair<,>), "System.KeyValuePair<,>")]
        [InlineData(typeof(IEnumerable<string>), "System.IEnumerable<String>")]
        [InlineData(typeof(IEnumerable<IEnumerable<string>>), "System.IEnumerable<IEnumerable<String>>")]
        [InlineData(typeof(KeyValuePair<bool, long>), "System.KeyValuePair<Boolean,Int64>")]
        [InlineData(typeof(KeyValuePair<KeyValuePair<bool, long>, KeyValuePair<bool, long>>), "System.KeyValuePair<KeyValuePair<Boolean,Int64>,KeyValuePair<Boolean,Int64>>")]
        public void PrettyPrint_Output_ShouldBeExpected(Type type, string expectedPrettyPrint)
        {
            var prettyPrint = type.PrettyPrint();
            
            prettyPrint.Should().Be(expectedPrettyPrint);
        }

        [Theory]
        [InlineData(typeof(FooAggregateWithOutAttribute), "FooAggregateWithOutAttribute")]
        [InlineData(typeof(FooAggregateWithAttribute), "BetterNameForAggregate")]
        public void AggregateName_FromType_ShouldBeExpected(Type aggregateType, string expectedAggregateName)
        {
            var aggregateName = aggregateType.GetAggregateName();
            
            aggregateName.Value.Should().Be(expectedAggregateName);
        }

        [Fact]
        public void AggregateName_WithNullString_Throws()
        {
            this.Invoking(test => new AggregateNameAttribute(null)).Should().Throw<ArgumentNullException>();
        }

        public class FooId : Identity<FooId>
        {
            public FooId(string value) : base(value)
            {
            }
        }

        public class FooAggregateWithOutAttribute : EventSourcedAggregateRoot<FooAggregateWithOutAttribute, FooId, FooStateWithOutAttribute>
        {
            public FooAggregateWithOutAttribute(FooId id) : base(id)
            {
            }
        }

        [AggregateName("BetterNameForAggregate")]
        public class FooAggregateWithAttribute : EventSourcedAggregateRoot<FooAggregateWithAttribute, FooId, FooStateWithAttribute>
        {
            public FooAggregateWithAttribute(FooId id) : base(id)
            {
            }
        }

        public class FooStateWithAttribute : AggregateState<FooAggregateWithAttribute, FooId,
            IMessageApplier<FooAggregateWithAttribute, FooId>>
        {

        }

        public class FooStateWithOutAttribute : AggregateState<FooAggregateWithOutAttribute, FooId,
            IMessageApplier<FooAggregateWithOutAttribute, FooId>>
        {
        }
    }
}
