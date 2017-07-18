using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class BackgroundJob : IJob
    {
        private readonly ILoggerFactory loggerFactory;

        public BackgroundJob(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var logger = loggerFactory.CreateLogger<BackgroundJob>();

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger.LogInformation("tock");

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}