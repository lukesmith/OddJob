using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace OddJob
{
    internal sealed class NullLoggerFactory : ILoggerFactory
    {
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
}