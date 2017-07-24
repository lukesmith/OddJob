using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob
{
    /// <summary>
    /// Represents a <see cref="IJobHost"/>.
    /// </summary>
    public class JobHost : IJobHost
    {
        private readonly IJob[] jobs;
        private readonly ILogger<JobHost> logger;

        /// <summary>
        /// Initializes a new <see cref="JobHost"/>.
        /// </summary>
        /// <param name="job">The job for the host to maintain.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to log to.</param>
        public JobHost(IJob job, ILoggerFactory loggerFactory)
            : this(new [] { job }, loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="JobHost"/>.
        /// </summary>
        /// <param name="jobs">The jobs for the host to maintain.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to log to.</param>
        public JobHost(IJob[] jobs, ILoggerFactory loggerFactory)
        {
            this.jobs = jobs;
            this.logger = loggerFactory.CreateLogger<JobHost>();
        }

        /// <inheritdoc />
        public IEnumerable<IJob> Jobs => this.jobs;

        /// <summary>
        /// Runs the <paramref name="job"/> to completion.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run.</param>
        /// <returns>Returns <value>true</value> if the <paramref name="job"/> completed before being cancelled;
        /// otherwise <value>false</value>.
        /// </returns>
        public static bool Run(IJob job)
        {
            return RunAsync(job).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the <paramref name="job"/> to completion, or for the
        /// duration specified by <paramref name="delayBeforeCancel"/>.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run.</param>
        /// <param name="delayBeforeCancel">
        /// A <see cref="TimeSpan"/> representing how long to wait for the <paramref name="job"/> to complete.
        /// </param>
        /// <returns>Returns <value>true</value> if the <paramref name="job"/> completed before being cancelled;
        /// otherwise <value>false</value>.
        /// </returns>
        public static bool Run(IJob job, TimeSpan delayBeforeCancel)
        {
            return RunAsync(job, delayBeforeCancel).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the <paramref name="job"/> to completion.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run.</param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that will be used to stop the run.
        /// </param>
        /// <returns>Returns <value>true</value> if the <paramref name="job"/> completed before being cancelled;
        /// otherwise <value>false</value>.
        /// </returns>
        public static async Task<bool> RunAsync(IJob job, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = false;

            var tasks = new[]
            {
                job.RunAsync(cancellationToken).ContinueWith(x => result = x.IsCompleted, cancellationToken),
            };

            await Task.WhenAny(tasks);

            return result;
        }

        /// <summary>
        /// Runs the <paramref name="job"/> to completion, or for the
        /// duration specified by <paramref name="delayBeforeCancel"/>.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run.</param>
        /// <param name="delayBeforeCancel">
        /// A <see cref="TimeSpan"/> representing how long to wait for the <paramref name="job"/> to complete.
        /// </param>
        /// <returns>Returns <value>true</value> if the <paramref name="job"/> completed before being cancelled;
        /// otherwise <value>false</value>.
        /// </returns>
        public static async Task<bool> RunAsync(IJob job, TimeSpan delayBeforeCancel)
        {
            using (var cancellationSource = new CancellationTokenSource())
            {
                return await RunAsync(job, delayBeforeCancel, cancellationSource.Token);
            }
        }

        /// <summary>
        /// Runs the <paramref name="job"/> to completion, or for the
        /// duration specified by <paramref name="delayBeforeCancel"/>.
        /// </summary>
        /// <param name="job">The <see cref="IJob"/> to run.</param>
        /// <param name="delayBeforeCancel">
        /// A <see cref="TimeSpan"/> representing how long to wait for the <paramref name="job"/> to complete.
        /// </param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> that will be used to stop the run.
        /// </param>
        /// <returns>Returns <value>true</value> if the <paramref name="job"/> completed before being cancelled;
        /// otherwise <value>false</value>.
        /// </returns>
        public static async Task<bool> RunAsync(IJob job, TimeSpan delayBeforeCancel, CancellationToken cancellationToken)
        {
            var result = false;

            var tasks = new[]
            {
                job.RunAsync(cancellationToken).ContinueWith(x => result = x.IsCompleted, cancellationToken),
                Task.Delay(delayBeforeCancel, cancellationToken)
            };

            await Task.WhenAny(tasks);

            return result;
        }

        /// <inheritdoc />
        public async Task StartAsync(CancellationTokenSource cts)
        {
            try
            {
                Task.WaitAll(this.jobs.Select(job => RunJobAsync(job, cts)
                    .ContinueWith(LogTaskCompletation, job)).ToArray());

                await Task.CompletedTask;
            }
            catch (AggregateException aex)
            {
                aex.Handle(ex => ex is OperationCanceledException);
            }
        }

        private void LogTaskCompletation(Task completedTask, object state)
        {
            var jobName = state.GetType().Name;

            if (completedTask.IsCanceled)
            {
                this.logger.LogInformation($"{jobName} was cancelled");
            }
            else if (completedTask.IsFaulted)
            {
                this.logger.LogInformation(0, completedTask.Exception, $"{jobName} faulted with message '{completedTask.Exception.Message}'");
                throw completedTask.Exception;
            }
            else
            {
                this.logger.LogInformation($"{jobName} has completed");
            }
        }

        private static async Task RunJobAsync(IJob job, CancellationTokenSource cts)
        {
            try
            {
                await job.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                if (!cts.IsCancellationRequested)
                {
                    cts.Cancel();
                }

                throw;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var job in this.jobs)
            {
                (job as IDisposable)?.Dispose();
            }
        }
    }
}