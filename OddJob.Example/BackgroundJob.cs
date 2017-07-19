using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class BackgroundJob : Jobs.Forever
    {
        private readonly ILoggerFactory loggerFactory;

        public BackgroundJob(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        protected override async Task DoAsync()
        {
            var logger = loggerFactory.CreateLogger<BackgroundJob>();

            logger.LogInformation("tock");

            await Task.Delay(1000);
        }
    }
}