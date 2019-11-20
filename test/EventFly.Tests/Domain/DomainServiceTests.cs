using Akka.TestKit.Xunit2;
using EventFly.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Domain
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class DomainServiceTests : TestKit
    {
        public DomainServiceTests(ITestOutputHelper testOutputHelper) : base(Configuration.Default, "DomainServiceTests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(new ServiceCollection().BuildServiceProvider());
        }

        [Fact]
        public void SuccessfulConstruction()
        {
            Action act = () => new DomainService();
            act.Should().NotThrow();
        }
    }
}