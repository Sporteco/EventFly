using Akka.TestKit.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using EventFly.Infrastructure.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace EventFly.Tests.Domain
{
    [Category(Categories.Domain)]
    [Collection(Collections.Only)]
    public class DomainServiceTests : TestKit
    {
        public DomainServiceTests(ITestOutputHelper testOutputHelper) : base(Tests.Configuration.Default, "DomainServiceTests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(new ServiceCollection().BuildServiceProvider());
        }

        [Fact]
        public void SuccessfulConstruction()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new Data.Domain.DomainService();
            act.Should().NotThrow();
        }
    }
}