using System;

namespace OddJob.Schedules
{
    /// <summary>
    /// Represents a <see cref="ISchedule"/> that runs daily.
    /// </summary>
    public sealed class Daily : ISchedule
    {
        private readonly TimeSpan timeOfDay;

#pragma warning disable SA1600 // Elements should be documented
        internal Daily()
#pragma warning restore SA1600 // Elements should be documented
            : this(TimeSpan.Zero)
        {
        }

#pragma warning disable SA1600 // Elements should be documented
        private Daily(TimeSpan timeOfDay)
#pragma warning restore SA1600 // Elements should be documented
        {
            this.timeOfDay = timeOfDay;
        }

        /// <summary>
        /// Creates a schedule which runs daily at the specified time.
        /// </summary>
        /// <param name="hour">A value representing the hour of the day.</param>
        /// <param name="minute">A value representing the minute of the <paramref name="hour"/>.</param>
        /// <param name="second">A value representing the second within the <paramref name="minute"/>.</param>
        /// <returns>A <see cref="Daily"/> schedule.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when either:
        ///     <paramref name="hour"/> is outside of 0-23
        ///     <paramref name="minute"/> is outside of 0-59
        ///     <paramref name="second"/> is outside of 0-59
        /// </exception>
        public ISchedule At(int hour, int minute, int second)
        {
            if (hour < 0 || hour > 23)
            {
                throw new ArgumentOutOfRangeException(nameof(hour), hour, "Value must be within range of 0-23");
            }

            if (minute < 0 || minute > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(minute), hour, "Value must be within range of 0-59");
            }

            if (second < 0 || second > 59)
            {
                throw new ArgumentOutOfRangeException(nameof(second), hour, "Value must be within range of 0-59");
            }

            return new Daily(new TimeSpan(hour, minute, second));
        }

        /// <inheritdoc />
        public DateTime Next(DateTime from)
        {
            return from.Date.AddDays(1).Add(this.timeOfDay);
        }
    }
}
