using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EventFly.Extensions;
using EventFly.Jobs;

namespace EventFly.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, JobName> JobNames = new ConcurrentDictionary<Type, JobName>();
        public static JobName GetJobName(
            this Type jobType)
        {
            return JobNames.GetOrAdd(
                jobType,
                t =>
                {
                    if (!typeof(IJob).GetTypeInfo().IsAssignableFrom(jobType))
                    {
                        throw new ArgumentException($"Type '{jobType.PrettyPrint()}' is not a job");
                    }

                    return new JobName(
                        t.GetTypeInfo().GetCustomAttributes<JobNameAttribute>().SingleOrDefault()?.Name ??
                        t.Name);
                });
        }

        internal static IReadOnlyList<Type> GetJobRunTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();
            var jobRunTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRun<,>))
                .Select(i => i.GetGenericArguments()[0])
                .ToList();


            return jobRunTypes;
        }

        internal static IReadOnlyList<Type> GetAsyncJobRunTypes(this Type type)
        {
            var interfaces = type
                .GetTypeInfo()
                .GetInterfaces()
                .Select(i => i.GetTypeInfo())
                .ToList();

            var jobRunTypes = interfaces
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRunAsync<,>))
                .Select(i => i.GetGenericArguments()[0])
                .ToList();

            return jobRunTypes;
        }

    }
}
