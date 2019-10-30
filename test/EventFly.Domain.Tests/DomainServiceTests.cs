using Akka.TestKit.Xunit2;
using EventFly.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Domain.Tests
{
    public class Service : DomainService<Service> { }

    public class DomainServiceTests : TestKit
    {
        public DomainServiceTests(ITestOutputHelper testOutputHelper) : base(TestHelpers.Configuration.Default, "DomainServiceTests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(new ServiceCollection().BuildServiceProvider());
        }

        [Fact]
        public void SuccessfulConstruction()
        {
            Action act = () => new Service();
            act.Should().NotThrow();
        }
    }
}