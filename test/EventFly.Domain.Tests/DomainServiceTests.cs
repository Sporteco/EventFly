using Akka.TestKit.Xunit2;
using EventFly.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using EventFly.DomainService;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Domain.Tests
{
    public class Service : AsynchronousDomainService<Service> { }

    public class DomainServiceTests : TestKit
    {
        public DomainServiceTests(ITestOutputHelper testOutputHelper) : base(TestHelpers.Configuration.Default, "DomainServiceTests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(new ServiceCollection().BuildServiceProvider());
        }

        [Fact]
        public void SuccessfulConstruction()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Service();
            act.Should().NotThrow();
        }
    }
}