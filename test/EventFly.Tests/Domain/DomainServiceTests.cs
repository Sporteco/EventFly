using Akka.TestKit.Xunit2;
using EventFly.DependencyInjection;
using EventFly.DomainService;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Domain
{
    public class Service : AsynchronousDomainService<Service> { }

    public class DomainServiceTests : TestKit
    {
        public DomainServiceTests(ITestOutputHelper testOutputHelper) : base(Configuration.Default, "DomainServiceTests", testOutputHelper)
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