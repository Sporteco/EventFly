using Akkatecture.Aggregates;
using Demo.Events;
using Demo.ValueObjects;

namespace Demo.Domain
{
    public class UserState : AggregateState<UserAggregate, UserId>,
        IApply<UserCreatedEvent>,
        IApply<UserRenamedEvent>
    {
        public UserName Name { get; private set; }
        public Birth Birth { get; private set; }

        public void Apply(UserCreatedEvent e)
        {
            Name = e.Name;
            Birth = e.Birth;
        }

        public void Apply(UserRenamedEvent e)
        {
            Name = e.NewName;
        }
    }
    
    public static class DbHelper
    {
        public static string ConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=akk-test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}