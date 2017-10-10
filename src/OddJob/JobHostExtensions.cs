using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    /// <summary>
    /// Extends <see cref="IJobHost"/> with extra functionality.
    /// </summary>
    public static class JobHostExtensions
    {
        /// <summary>
        /// Runs the jobs.
        /// </summary>
        /// <param name="host">The host whose jobs should be run.</param>
        public static void Run(this IJobHost host)
        {
            host.RunAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the jobs.
        /// </summary>
        /// <param name="host">The host whose jobs should be run.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to use that will cancel the running jobs.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task RunAsync(this IJobHost host, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (cancellationToken.CanBeCanceled)
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
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
                    Console.WriteLine($"Running job - {job.GetName()}");
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
