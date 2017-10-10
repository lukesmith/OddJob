using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OddJob.Schedules;

namespace OddJob.Jobs
{
    /// <summary>
    /// Represents a <see cref="IJob"/> that runs on a schedule, or
    /// until the host process ends.
    /// </summary>
    internal class ScheduledJob : IJob, IDisposable
    {
        private readonly IJob job;
        private readonly ILogger logger;
        private readonly ISchedule schedule;
        private readonly IClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledJob"/> class.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run on the <paramref name="schedule"/>.</param>
        /// <param name="schedule">The <see cref="ISchedule"/> to use to run the <paramref name="job"/>.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/>.</param>
        /// <param name="clock">A reference to the <see cref="IClock"/> to use for scheduling.</param>
        public ScheduledJob(IJob job, ISchedule schedule, ILoggerFactory loggerFactory, IClock clock)
        {
            this.job = job;
            this.logger = loggerFactory.CreateLogger<ScheduledJob>();
            this.schedule = schedule;
            this.clock = clock;
        }

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var jobType = this.job.GetType().Name;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var next = this.schedule.Next(this.clock.UtcNow);

                var now = this.clock.UtcNow;
                if (next >= now)
                {
                    var delay = next - now;

                    this.logger.LogInformation("Waiting for {0} until {1}", delay, next);

                    await Task.Delay(delay, cancellationToken);

                    this.logger.LogInformation("Starting job {0}", jobType);

                    this.job.RunAsync(cancellationToken).Wait(cancellationToken);

                    this.logger.LogInformation("Worker job {0} completed", jobType);
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (this.job is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
