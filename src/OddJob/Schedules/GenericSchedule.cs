using System;

namespace OddJob.Schedules
{
#pragma warning disable SA1600 // Elements should be documented
    internal class GenericSchedule : ISchedule
    {
        private readonly Func<DateTime, DateTime> func;

        public GenericSchedule(Func<DateTime, DateTime> func)
        {
            this.func = func;
        }

        public static implicit operator GenericSchedule(Func<DateTime, DateTime> a)
        {
            return new GenericSchedule(a);
        }

        public DateTime Next(DateTime from)
        {
            return this.func(from);
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
