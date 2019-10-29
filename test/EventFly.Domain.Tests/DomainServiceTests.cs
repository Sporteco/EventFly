using Akka.TestKit.Xunit2;
using EventFly.Core;
using EventFly.DependencyInjection;
using EventFly.DomainService;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Domain.Tests
{
    public class Service : DomainService<Service, ServiceId> { }
    public class ServiceId : Identity<ServiceId> { public ServiceId(String value) : base(value) { } }

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