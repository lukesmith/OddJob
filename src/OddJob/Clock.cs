using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OddJob.Tests")]

namespace OddJob
{
#pragma warning disable SA1600 // Elements should be documented
    internal sealed class Clock : IClock
    {
        public static readonly IClock DefaultClock = new Clock();

        private readonly Func<DateTime> get = () => DateTime.UtcNow;

        public Clock()
        {
        }

        public Clock(Func<DateTime> timeFunc)
        {
            this.get = timeFunc;
        }

        public DateTime UtcNow => this.get();
    }
#pragma warning restore SA1600 // Elements should be documented
}
