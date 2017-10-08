using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class OneTimeJob : IJob
    {
        private readonly ILoggerFactory loggerFactory;

        public OneTimeJob(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var logger = this.loggerFactory.CreateLogger<OneTimeJob>();

            logger.LogInformation("Thats it");

            await Task.CompletedTask;
        }
    }
}
