﻿using Akka.TestKit.Xunit2;
using Demo.Commands;
using Demo.Domain.Aggregates;
using Demo.Events;
using Demo.Infrastructure;
using EventFly.Aggregates;
using EventFly.Commands;
using EventFly.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class UserTouchTrackingServiceTests : TestKit
    {
        public UserTouchTrackingServiceTests(ITestOutputHelper testOutputHelper) : base(Configuration.Config, "TestDomainServiceTests", testOutputHelper)
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
            //Sys.EventStream.Subscribe(events, typeof(DomainEvent<UserAggregate, UserId, UserNotesChangedEvent>));
            Sys.EventStream.Subscribe(events, typeof(DomainEvent<UserAggregate, UserId, UserTouchedEvent>));
            var bus = Sys.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>();

            var changeUserNotes = new ChangeUserNotesCommand(UserId.New, "Such a nice user...");
            bus.Publish(changeUserNotes).GetAwaiter().GetResult();

            events.ExpectMsg<DomainEvent<UserAggregate, UserId, UserTouchedEvent>>();
        }
    }
}