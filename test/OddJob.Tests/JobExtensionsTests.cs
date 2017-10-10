using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OddJob.Tests
{
    public class JobExtensionsTests
    {
        [Fact]
        public void ClassNotSpecifyingTheNameAttribute()
        {
            var result = JobExtensions.GetName(typeof(NoNameAttribute));
            Assert.Equal("NoNameAttribute", result);
        }

        [Fact]
        public void ClassSpecifyingTheNameAttributeWithAName()
        {
            var result = JobExtensions.GetName(typeof(NameAttributeWithName));
            Assert.Equal("HelloWorld", result);
        }

        private class NoNameAttribute : IJob
        {
            public Task RunAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }

        [Name("HelloWorld")]
        private class NameAttributeWithName : IJob
        {
            public Task RunAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
