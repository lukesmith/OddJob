using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    public class BackgroundJob : Jobs.Forever
    {
        private readonly ILogger logger;

        public BackgroundJob(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<BackgroundJob>();
        }

        protected override async Task DoAsync()
        {
            this.logger.LogInformation("tock");

            await Task.Delay(1000);
        }

        protected override void OnCancel()
        {
            this.logger.LogInformation("Cancelling job");
        }
    }
}