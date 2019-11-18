using EventFly.Core;
using FluentAssertions;
using System;
using Xunit;

namespace EventFly.Tests.UnitTests.Core
{
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