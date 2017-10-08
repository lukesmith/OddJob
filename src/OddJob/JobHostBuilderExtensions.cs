using System;

namespace OddJob
{
    /// <summary>
    /// Extends the <see cref="JobHostBuilder"/>.
    /// </summary>
    public static class JobHostBuilderExtensions
    {
        /// <summary>
        /// Builds and runs the jobs.
        /// </summary>
        /// <param name="builder">The <see cref="JobHostBuilder"/> to build and run.</param>
        /// <returns>
        /// An <see cref="int"/> representing the completion state of the jobs. zero = completed
        /// successfully; non-zero = an error.
        /// </returns>
        public static int BuildAndRun(this JobHostBuilder builder)
        {
            using (var host = builder.Build())
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Gray;

                    host.Run();
                }
                catch (AggregateException aex)
                {
                    foreach (var ex in aex.Flatten().InnerExceptions)
                    {
                        if (ex is OperationCanceledException)
                        {
                        }
                        else
                        {
                            WriteException(ex);
                        }
                    }

                    return 1;
                }
                catch (Exception ex)
                {
                    WriteException(ex);
                    return 1;
                }
                finally
                {
                    Console.ResetColor();
                }
            }

            return 0;
        }

        private static void WriteException(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }
    }
}
