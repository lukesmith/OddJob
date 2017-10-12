using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OddJob
{
#pragma warning disable SA1600 // Elements should be documented
    internal sealed class NullLoggerFactory : ILoggerFactory
    {
        public static readonly NullLoggerFactory Instance = new NullLoggerFactory();

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return NullLoggerProvider.Instance.CreateLogger(categoryName);
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
#pragma warning restore SA1600 // Elements should be documented
}
