using Akka.TestKit.Xunit2;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Events;
using Demo.Infrastructure;
using Demo.ValueObjects;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class TestDomainServiceTests : TestKit
    {
        public TestDomainServiceTests(ITestOutputHelper testOutputHelper) : base(Configuration.Config, "TestDomainServiceTests", testOutputHelper)
        {
            Sys.RegisterDependencyResolver(
                new ServiceCollection()
                .AddEventFly(Sys)
                .WithContext<UserContext>()
                .Services
                .BuildServiceProvider()
                .UseEventFly()
            );
        }

        [Fact]
        public void RegisteredDomainServiceWorking()
        {
            var events = CreateTestProbe();
            Sys.EventStream.Subscribe(events, typeof(DomainEvent<UserAggregate, UserId, UserRenamedEvent>));
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            var createUser = new CreateUserCommand(UserId.New, new UserName("name"), new Birth(DateTime.MinValue));
            bus.Publish(createUser).GetAwaiter().GetResult();

            events.ExpectMsg<DomainEvent<UserAggregate, UserId, UserRenamedEvent>>();
        }
    }
}