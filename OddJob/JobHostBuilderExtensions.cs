using System;

namespace OddJob
{
    public static class JobHostBuilderExtensions
    {
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