using System;
using System.Linq;
using System.Reflection;

namespace OddJob
{
#pragma warning disable SA1600 // Elements should be documented
    internal static class JobExtensions
    {
        public static string GetName(this IJob job)
        {
            return GetName(job.GetType());
        }

        public static string GetName(Type jobType)
        {
            var attribute = jobType.GetTypeInfo().GetCustomAttributes<NameAttribute>().FirstOrDefault();
            if (attribute == null)
            {
                return jobType.Name;
            }

            return attribute.GetName();
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
