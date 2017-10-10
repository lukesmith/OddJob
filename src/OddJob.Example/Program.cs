using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OddJob.Schedules;

namespace OddJob.Example
{
    public class Program
    {
        private static int Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .Build();

            using (var loggerFactory = new LoggerFactory())
            {
                loggerFactory.AddConsole();

                var builder = new JobHostBuilder()
                    .Add(() => new OneTimeJob(loggerFactory), Schedule.Every().Second())
                    .Add(() => new OneTimeJob(loggerFactory), Schedule.Every().Minute())
                    .Add(() => new OneTimeJob(loggerFactory), Schedule.Every().Hour())
                    .Add(() => new OneTimeJob(loggerFactory), Schedule.Every().Day().At(12, 3, 1))
                    .Add(() => new OneTimeJob(loggerFactory))
                    .Add(() => new FailingBackgroundJob(loggerFactory))
                    .Add(() => new BackgroundJob(loggerFactory))
                    .Add(() => new WebServer(configuration, loggerFactory))
                    .UseLoggerFactory(loggerFactory);

                return builder.BuildAndRun();
            }
        }
    }
}
