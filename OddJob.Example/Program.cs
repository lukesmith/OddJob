using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace OddJob.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            using (var loggerFactory = new LoggerFactory())
            {
                loggerFactory.AddConsole();

                var builder = new JobHostBuilder()
                    .Add(() => new OneTimeJob(loggerFactory))
                    .Add(() => new FailingBackgroundJob(loggerFactory))
                    .Add(() => new BackgroundJob(loggerFactory))
                    .Add(() => new WebServer(configuration, loggerFactory));

                return builder.BuildAndRun();
            }
        }
    }
}