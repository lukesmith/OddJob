namespace OddJob.Schedules
{
    /// <summary>
    /// Contains builders for creating <see cref="ISchedule"/>
    /// </summary>
    public static class Schedule
    {
        /// <summary>
        /// Create a schedule that runs on a fix period of time.
        /// </summary>
        /// <returns>
        /// A reference to <see cref="IEvery"/>.
        /// </returns>
        public static IEvery Every() => new EveryImpl();

        private sealed class EveryImpl : IEvery
        {
        }
    }
}
