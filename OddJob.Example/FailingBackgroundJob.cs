using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class FailingBackgroundJob : IJob
    {
        private readonly ILoggerFactory loggerFactory;

        public FailingBackgroundJob(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            int i = 0;

            var logger = this.loggerFactory.CreateLogger<FailingBackgroundJob>();
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger.LogInformation("tick");

                if (i == 1)
                {
                    throw new NotImplementedException("Failing background job");
                }

                i++;

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}