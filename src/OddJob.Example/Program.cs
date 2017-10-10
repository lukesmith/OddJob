using System;
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
                    .Add(() => new OneTimeJob(loggerFactory), new EveryTenSeconds())
                    .Add(() => new OneTimeJob(loggerFactory))
                    .Add(() => new FailingBackgroundJob(loggerFactory))
                    .Add(() => new BackgroundJob(loggerFactory))
                    .Add(() => new WebServer(configuration, loggerFactory))
                    .UseLoggerFactory(loggerFactory);

                return builder.BuildAndRun();
            }
        }

        private class EveryTenSeconds : ISchedule
        {
            public DateTime Next(DateTime from)
            {
                var a = from.Second % 10;
                return from.AddSeconds(10 - a);
            }
        }
    }
}
