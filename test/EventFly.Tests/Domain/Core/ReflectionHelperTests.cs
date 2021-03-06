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
using EventFly.Core;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.Domain.Core
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class ReflectionHelperTests
    {
        [Fact]
        public void CompileMethodInvocation()
        {
            var caller = ReflectionHelper.CompileMethodInvocation<Func<Calculator, Int32, Int32, Int32>>(typeof(Calculator), "Add", typeof(Int32), typeof(Int32));

            var result = caller(new Calculator(), 1, 2);

            result.Should().Be(3);
        }

        [Fact]
        public void CompileMethodInvocation_CanUpcast()
        {
            var a = (INumber)new Number { I = 1 };
            var b = (INumber)new Number { I = 2 };

            var caller = ReflectionHelper.CompileMethodInvocation<Func<Calculator, INumber, INumber, INumber>>(typeof(Calculator), "Add", typeof(Number), typeof(Number));
            var result = caller(new Calculator(), a, b);

            var c = (Number)result;
            c.I.Should().Be(3);
        }

        [Fact]
        public void CompileMethodInvocation_CanDoBothUpcastAndPass()
        {
            var a = (INumber)new Number { I = 1 };
            const Int32 b = 2;

            var caller = ReflectionHelper.CompileMethodInvocation<Func<Calculator, INumber, Int32, INumber>>(typeof(Calculator), "Add", typeof(Number), typeof(Int32));
            var result = caller(new Calculator(), a, b);

            var c = (Number)result;
            c.I.Should().Be(3);
        }

        [Fact]
        public void CompileMethodInvocation_WithMissingMethod_ThrowsException()
        {
            var methodName = "ThisMethodDoesntExist";

            this.Invoking(test => ReflectionHelper.CompileMethodInvocation<Func<Calculator, INumber, Int32, INumber>>(typeof(Calculator), methodName, typeof(Number), typeof(Int32)))
                .Should().Throw<ArgumentException>().And.Message.Should().Contain(methodName);
        }

        public interface INumber { }

        public class Number : INumber
        {
            public Int32 I { get; set; }
        }

        public class Calculator
        {
            public Int32 Add(Int32 a, Int32 b) => a + b;
            private Number Add(Number a, Number b) => new Number { I = Add(a.I, b.I) };
            private Number Add(Number a, Int32 b) => new Number { I = Add(a.I, b) };
        }
    }
}