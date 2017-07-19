using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

namespace OddJob.Tests
{
    public class JobHostTests
    {
        [Fact]
        public async Task StartingAJobThatInstantlyFails()
        {
            var tracerId = Guid.NewGuid().ToString();
            var jobs = new IJob[] { new InstantlyFailingJob(tracerId), };

            using (var loggerFactory = new LoggerFactory())
            {
                var host = new JobHost(jobs, loggerFactory);

                using (var cts = new CancellationTokenSource())
                {
                    var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await host.StartAsync(cts);
                    });

                    Assert.Equal(tracerId, ex.Flatten().InnerExceptions.Single(x => x is TestException).Message);
                    Assert.True(cts.IsCancellationRequested,
                        "A job that fails to start instantly should request a cancellation");
                }
            }
        }

        [Fact]
        public async Task StartingAJobThatFailsAfterADelay()
        {
            var tracerId = Guid.NewGuid().ToString();
            var jobs = new IJob[] { new DelayedFailingJob(tracerId), };

            using (var loggerFactory = new LoggerFactory())
            {
                var host = new JobHost(jobs, loggerFactory);

                using (var cts = new CancellationTokenSource())
                {
                    var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await host.StartAsync(cts);
                    });

                    Assert.Equal(tracerId, ex.Flatten().InnerExceptions.Single(x => x is TestException).Message);
                    Assert.True(cts.IsCancellationRequested,
                        "A job that fails after a delay should request a cancellation");
                }
            }
        }

        [Fact]
        public async Task AllJobsCancelledOnSingleException()
        {
            var tracerId = Guid.NewGuid().ToString();
            var jobs = new IJob[] { new ForeverJob(),  new InstantlyFailingJob(tracerId), };

            using (var loggerFactory = new LoggerFactory())
            {
                var host = new JobHost(jobs, loggerFactory);

                using (var cts = new CancellationTokenSource())
                {
                    var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
                    {
                        await host.StartAsync(cts);
                    });

                    Assert.Equal(tracerId, ex.Flatten().InnerExceptions.Single(x => x is TestException).Message);
                    Assert.True(cts.IsCancellationRequested,
                        "A job that fails should request a cancellation");
                }
            }
        }

        private sealed class ForeverJob : IJob
        {
            public async Task RunAsync(CancellationToken token)
            {
                while (true)
                {
                    await Task.Delay(100, token);
                }
            }
        }

        private sealed class InstantlyFailingJob : IJob
        {
            private readonly string exceptionMessage;

            public InstantlyFailingJob(string exceptionMessage)
            {
                this.exceptionMessage = exceptionMessage;
            }

            public Task RunAsync(CancellationToken token)
            {
                throw new TestException(this.exceptionMessage);
            }
        }

        private sealed class DelayedFailingJob : IJob
        {
            private readonly string exceptionMessage;

            public DelayedFailingJob(string exceptionMessage)
            {
                this.exceptionMessage = exceptionMessage;
            }

            public async Task RunAsync(CancellationToken token)
            {
                await Task.Delay(1000, token);

                throw new TestException(this.exceptionMessage);
            }
        }

        public class TestException : Exception
        {
            public TestException()
            {
            }

            public TestException(string message) : base(message)
            {
            }

            public TestException(string message, Exception inner) : base(message, inner)
            {
            }
        }
    }
}
