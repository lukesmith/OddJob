using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob
{
    public class JobHost : IJobHost
    {
        private readonly IJob[] jobs;
        private readonly ILogger<JobHost> logger;

        public JobHost(IJob[] jobs, ILoggerFactory loggerFactory)
        {
            this.jobs = jobs;
            this.logger = loggerFactory.CreateLogger<JobHost>();
        }

        public IEnumerable<IJob> Jobs => this.jobs;

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

        public void Dispose()
        {
        }
    }
}