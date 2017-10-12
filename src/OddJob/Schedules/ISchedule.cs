using System;

namespace OddJob.Schedules
{
    /// <summary>
    /// Defines the interface for a <see cref="IJob"/> schedule.
    /// </summary>
    public interface ISchedule
    {
        /// <summary>
        /// Gets the next <see cref="DateTime"/> in the schedule after the <paramref name="from"/> value specified.
        /// </summary>
        /// <param name="from">A <see cref="DateTime"/>.</param>
        /// <returns>The next <see cref="DateTime"/> in the schedule.</returns>
        DateTime Next(DateTime from);
    }
}
