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

using EventFly.Specifications.Provided;
using FluentAssertions;
using System;
using System.ComponentModel;
using Xunit;

namespace EventFly.Tests.Domain
{
    [Category(Categories.Domain)]
    public class ExpressionSpecificationTests
    {
        [Fact]
        public void StringIsRight()
        {
            var specification = new ExpressionSpecification<Int32>(i => (i > 1 && i < 10) || i == 42);

            var str = specification.ToString();

            str.Should().Be("i => (((i > 1) && (i < 10)) || (i == 42))");
        }

        [Theory]
        [InlineData(42, true)]
        [InlineData(-42, false)]
        public void ExpressionIsEvaluated(Int32 value, Boolean expectedIsSatisfied)
        {
            var is42 = new ExpressionSpecification<Int32>(i => i == 42);

            var result = is42.IsSatisfiedBy(value);

            result.Should().Be(expectedIsSatisfied);
        }
    }
}