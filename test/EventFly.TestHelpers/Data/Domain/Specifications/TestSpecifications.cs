// The MIT License (MIT)
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

using EventFly.Specifications;
using System;
using System.Collections.Generic;

namespace EventFly.Tests.Domain
{
    public static class TestSpecifications
    {
        public class IsAboveSpecification : Specification<Int32>
        {
            public IsAboveSpecification(Int32 limit)
            {
                _limit = limit;
            }

            protected override IEnumerable<String> IsNotSatisfiedBecause(Int32 account)
            {
                if (account <= _limit)
                {
                    yield return $"{account} is less or equal than {_limit}";
                }
            }

            private readonly Int32 _limit;
        }

        public class IsTrueSpecification : Specification<Boolean>
        {
            protected override IEnumerable<String> IsNotSatisfiedBecause(Boolean account)
            {
                if (!account) yield return "Its false!";
            }
        }
    }
}