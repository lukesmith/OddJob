using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    public static class JobHostExtensions
    {
        public static void Run(this IJobHost runner)
        {
            runner.RunAsync().GetAwaiter().GetResult();
        }

        public static async Task RunAsync(this IJobHost host, CancellationToken token = default(CancellationToken))
        {
            if (token.CanBeCanceled)
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    await host.RunImplAsync(cts);
                }
            }
            else
            {
                var done = new ManualResetEventSlim(false);

                using (var cts = new CancellationTokenSource())
                {
                    AttachCtrlcSigtermShutdown(cts, done);

                    Console.WriteLine("Application started. Press Ctrl+C to shut down.");

                    try
                    {
                        await host.RunImplAsync(cts);
                    }
                    catch (Exception ex)
                    {
                        await Task.FromException(ex);
                    }
                    finally
                    {
                        done.Set();
                    }
                }
            }
        }

        private static async Task RunImplAsync(this IJobHost host, CancellationTokenSource cts)
        {
            using (host)
            {
                foreach (var job in host.Jobs)
                {
                    Console.WriteLine($"Running job - {job.GetType().Name}");
                }

                await host.StartAsync(cts);
            }
        }

        private static void AttachCtrlcSigtermShutdown(CancellationTokenSource cts, ManualResetEventSlim manualAwait)
        {
            AssemblyLoadContext.Default.Unloading += sender => Shutdown(cts, manualAwait);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Shutdown(cts, manualAwait);

                // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                eventArgs.Cancel = true;
            };
        }

        private static void Shutdown(CancellationTokenSource cts, ManualResetEventSlim manualAwait)
        {
            if (!cts.IsCancellationRequested)
            {
                Console.WriteLine("Application is shutting down...");

                try
                {
                    cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
            }

            manualAwait.Wait();
        }
    }
}