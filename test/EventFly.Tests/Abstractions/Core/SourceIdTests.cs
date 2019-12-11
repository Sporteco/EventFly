using System;
using System.ComponentModel;
using EventFly.Core;
using FluentAssertions;
using Xunit;

namespace EventFly.Tests.Abstractions.Core
{
    [Category(Categories.Abstractions)]
    [Collection(Collections.Only)]
    public class SourceIdTests
    {
        [Fact]
        public void InstantiatingSourceId_WithNullString_ThrowsException()
        {
            this.Invoking(test => new SourceId(null))
                .Should().Throw<ArgumentNullException>();
        }
    }
}