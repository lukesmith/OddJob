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
                Task.WaitAll(this.jobs.Select(t => RunJobAsync(t, cts)).ToArray());

                await Task.CompletedTask;
            }
            catch (AggregateException aex)
            {
                aex.Handle(ex => ex is OperationCanceledException);
            }
        }

        private async Task RunJobAsync(IJob job, CancellationTokenSource cts)
        {
            try
            {
                await job.RunAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                this.logger.LogInformation($"{job.GetType().Name} was cancelled");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogInformation(
                    0,
                    ex,
                    $"{job.GetType().Name} has halted with message '{ex.Message}'");

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