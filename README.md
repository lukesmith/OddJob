# OddJob

[![Build status](https://ci.appveyor.com/api/projects/status/k6vu2qps4akst6cj/branch/master?svg=true)](https://ci.appveyor.com/project/lukesmith/oddjob/branch/master)
[![NuGet](http://img.shields.io/nuget/v/OddJob.svg)](https://www.nuget.org/packages/OddJob/)

OddJob was born out of the need to run both a long running process and a HttpServer within a single command line application,
along with handling graceful shutdown of the process when signalled by the OS.

Unhandled exceptions by any job will signal a cancellation to all jobs to gracefully end, eventually exiting the process.

## Example

```csharp
class Program
{
    static int Main(string[] args)
    {
        using (var loggerFactory = new LoggerFactory())
        {
            loggerFactory.AddConsole();

            var builder = new OddJob.JobHostBuilder()
                .Add<BackgroundJob>()
                .UseLoggerFactory(loggerFactory);

            return builder.BuildAndRun();
        }
    }
}

public class BackgroundJob : OddJob.Jobs.Forever
{
    protected override async Task DoAsync()
    {
        Console.WriteLine("tick");

        await Task.Delay(1000);
    }
}
```