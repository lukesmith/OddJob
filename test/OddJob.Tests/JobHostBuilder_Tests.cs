using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OddJob.Tests
{
    public class JobHostBuilder_Tests
    {
        [Fact]
        public void AddInstance()
        {
            var job = new TestJob();
            var builder = new JobHostBuilder();
            builder.Add(job);

            var host = builder.Build();

            Assert.Equal(new[] { job }, host.Jobs);
        }

        [Fact]
        public void AddFactory()
        {
            var job = new TestJob();
            var builder = new JobHostBuilder();
            builder.Add(() => job);

            var host = builder.Build();

            Assert.Equal(new[] { job }, host.Jobs);
        }

        [Fact]
        public void AddGeneric()
        {
            var builder = new JobHostBuilder();
            builder.Add<TestJob>();

            var host = builder.Build();

            Assert.Single(host.Jobs);
        }

        private class TestJob : IJob
        {
            public async Task RunAsync(CancellationToken cancellationToken)
            {
                await Task.CompletedTask;
            }
        }
    }
}
