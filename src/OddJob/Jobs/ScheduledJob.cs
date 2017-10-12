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
    /// <typeparam name="T">The type of <see cref="IJob"/> that will be run on the schedule.</typeparam>
    [Name(nameof(ScheduledJob<T>))]
    internal class ScheduledJob<T> : IJob
        where T : IJob
    {
        private readonly Func<T> jobFactory;
        private readonly ISchedule schedule;
        private readonly ILoggerFactory loggerFactory;
        private readonly IClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledJob{T}"/> class.
        /// </summary>
        /// <param name="jobFactory">A factory to create the <see cref="IJob"/> to run on the <paramref name="schedule"/>.</param>
        /// <param name="schedule">The <see cref="ISchedule"/> to use to run the <see cref="IJob"/>.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/>.</param>
        /// <param name="clock">A reference to the <see cref="IClock"/> to use for scheduling.</param>
        public ScheduledJob(Func<T> jobFactory, ISchedule schedule, ILoggerFactory loggerFactory, IClock clock)
        {
            this.jobFactory = jobFactory;
            this.schedule = schedule;
            this.loggerFactory = loggerFactory;
            this.clock = clock;
        }

        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
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

                    var job = this.jobFactory();
                    var jobName = job.GetName();

                    var logger = this.loggerFactory.CreateLogger($"{this.GetName()}:{jobName}");
                    logger.LogInformation("Waiting for {0} until {1}", delay, next);

                    await Task.Delay(delay, cancellationToken);

                    logger.LogInformation("Starting job {0}", jobName);

                    try
                    {
                        var taskState = new TaskState
                        {
                            Job = job,
                            Logger = logger,
                        };

                        job.RunAsync(cancellationToken)
                            .ContinueWith((task, state) => LogTaskCompletation(task, state as TaskState), taskState, cancellationToken)
                            .Wait(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogInformation(0, ex, $"{jobName} errored with message '{ex.Message}'");
                    }
                    finally
                    {
                        if (job is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }

                    logger.LogInformation("Worker job {0} completed", jobName);
                }
            }
        }

        private static void LogTaskCompletation(Task completedTask, TaskState state)
        {
            var jobName = state.Job.GetName();

            if (completedTask.IsCanceled)
            {
                state.Logger.LogInformation($"{jobName} was cancelled");
            }
            else if (completedTask.IsFaulted)
            {
                state.Logger.LogInformation(0, completedTask.Exception, $"{jobName} faulted with message '{completedTask.Exception.Message}'");
            }
            else
            {
                state.Logger.LogInformation($"{jobName} has completed");
            }
        }

        private class TaskState
        {
            public IJob Job { get; set; }

            public ILogger Logger { get; set; }
        }
    }
}
