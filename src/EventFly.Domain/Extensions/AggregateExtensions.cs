using EventFly.Aggregates;
using EventFly.Commands.ExecutionResults;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace EventFly.Extensions
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
                var funcType = typeof(Func<,>).MakeGenericType(subscriptionType.Item1, typeof(Task<IExecutionResult>));
                var subscriptionFunction = Delegate.CreateDelegate(funcType, aggregate, methods[subscriptionType.Item1]);
                var actorReceiveMethod = method.MakeGenericMethod(subscriptionType.Item1, typeof(IExecutionResult));

                actorReceiveMethod.Invoke(aggregate, new Object[] { subscriptionFunction });
            }
        }
    }
}