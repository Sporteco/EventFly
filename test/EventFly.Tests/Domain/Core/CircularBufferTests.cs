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
using System.ComponentModel;
using System.Linq;
using EventFly.Core;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.Domain.Core
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class CircularBufferTests
    {
        [Theory]
        [InlineData(1)] // Below capacity
        [InlineData(1, 2)] // At capacity
        [InlineData(1, 2, 3)] // Once above capacity
        [InlineData(1, 2, 3, 4)] // Loop twice over capacity
        [InlineData(1, 2, 3, 4, 5)] // One more than of capacity
        public void CircularBuffer_Putting_ShouldContainCapacity(params Int32[] numbers)
        {
            const Int32 capacity = 2;
            var sut = new CircularBuffer<Int32>(capacity);

            foreach (var number in numbers) sut.Put(number);

            var shouldContain = numbers.Reverse().Take(capacity).ToList();
            sut.Should().Contain(shouldContain);
        }

        [Theory]
        [InlineData(1)] // Below capacity
        [InlineData(1, 2)] // At capacity
        [InlineData(1, 2, 3)] // Once above capacity
        [InlineData(1, 2, 3, 4)] // Loop twice over capacity
        [InlineData(1, 2, 3, 4, 5)] // One more than of capacity
        public void CircularBuffer_PuttingArray_ShouldContainNumbers(params Int32[] numbers)
        {
            const Int32 capacity = 10;
            var circularBuffer = new CircularBuffer<Int32>(capacity, numbers);

            var shouldContain = numbers.Reverse().Take(capacity).ToList();
            circularBuffer.Should().Contain(shouldContain);
        }

        [Theory]
        [InlineData(1, 2, 6)] // At capacity
        [InlineData(1, 2, 3, 7)] // Once above capacity
        [InlineData(1, 2, 3, 4, 9)] // Loop twice over capacity
        [InlineData(1, 2, 3, 4, 5, 10)] // One more than of capacity
        public void CircularBuffer_PuttingArrayWithSmallCapacity_ThrowsException(params Int32[] numbers)
        {
            this.Invoking(test => new CircularBuffer<Int32>(2, numbers))
                .Should().Throw<ArgumentException>();
        }
    }
}