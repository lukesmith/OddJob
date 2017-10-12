using System;

namespace OddJob.Schedules
{
    /// <summary>
    /// Extensions for <see cref="IEvery"/>.
    /// </summary>
    public static class EveryExtensions
    {
        /// <summary>
        /// Creates a schedule which runs daily at the specified time.
        /// </summary>
        /// <param name="every">An <see cref="IEvery"/> instance.</param>
        /// <returns>A <see cref="Daily"/> schedule.</returns>
        public static Daily Day(this IEvery every) => new Daily();

        /// <summary>
        /// Creates a <see cref="ISchedule"/> of every hour.
        /// </summary>
        /// <param name="every">A <see cref="IEvery"/>.</param>
        /// <returns>
        /// A <see cref="ISchedule"/> of every hour.
        /// </returns>
        public static ISchedule Hour(this IEvery every) =>
            new GenericSchedule(from => from.Date.Add(new TimeSpan(from.Hour, 0, 0)).AddHours(1));

        /// <summary>
        /// Creates a <see cref="ISchedule"/> of every minute.
        /// </summary>
        /// <param name="every">A <see cref="IEvery"/>.</param>
        /// <returns>
        /// A <see cref="ISchedule"/> of every minute.
        /// </returns>
        public static ISchedule Minute(this IEvery every) => new GenericSchedule(from =>
            from.Date.Add(new TimeSpan(from.Hour, from.Minute, 0)).AddMinutes(1));

        /// <summary>
        /// Creates a <see cref="ISchedule"/> of every second.
        /// </summary>
        /// <param name="every">A <see cref="IEvery"/>.</param>
        /// <returns>
        /// A <see cref="ISchedule"/> of every second.
        /// </returns>
        public static ISchedule Second(this IEvery every) =>
            new GenericSchedule(from => from.Date.Add(new TimeSpan(from.Hour, from.Minute, from.Second)).AddSeconds(1));
    }
}
