using System;
using System.Linq;
using System.Reflection;
using Akka.Actor;
using Akka.Event;
using Akkatecture.Aggregates;
using Akkatecture.ReadModels;

namespace Akkatecture.Extensions
{
    internal static class AggregateExtensions
    {
        public static void InitAggregateReceivers(this IAggregateRoot aggregate)
        {
            var type = aggregate.GetType();
            
            var subscriptionTypes =
                type.GetAggregateExecuteTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Execute") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1;
                })
                .ToDictionary(
                    mi => mi.GetParameters()[0].ParameterType,
                    mi => mi);
            
            var method = type
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Command") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1
                        && parameters[0].ParameterType.Name.Contains("Func");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType.Item1, subscriptionType.Item2);
                var subscriptionFunction = Delegate.CreateDelegate(funcType, aggregate, methods[subscriptionType.Item1]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType.Item1,subscriptionType.Item2);

                actorReceiveMethod.Invoke(aggregate, new object[] { subscriptionFunction });
            }
        }
        public static void InitReadModelReceivers(this IReadModel readModel, EventStream eventStream, IActorRef subscriberRef)
        {

            var type = readModel.GetType();

            var subscriptionTypes =
                type.GetReadModelSubscribersTypes();

            var methods = type
                .GetTypeInfo()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Apply") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 2;
                })
                .ToDictionary(
                    mi => mi.GetParameters()[1].ParameterType,
                    mi => mi);
            
            var method = type
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(mi =>
                {
                    if (mi.Name != "Command") return false;
                    var parameters = mi.GetParameters();
                    return
                        parameters.Length == 1
                        && parameters[0].ParameterType.Name.Contains("Action");
                })
                .First();

            foreach (var subscriptionType in subscriptionTypes)
            {
                var domainEventType = typeof(IDomainEvent<,>).MakeGenericType(subscriptionType.Item1, subscriptionType.Item2);
                var funcType = typeof(Action<,>).MakeGenericType(typeof(IReadModelContext), domainEventType);
                var subscriptionFunction = Delegate.CreateDelegate(funcType, readModel, methods[domainEventType]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType.Item1,subscriptionType.Item2);

                actorReceiveMethod.Invoke(readModel, new object[] { subscriptionFunction });
            }
        }
    }
}