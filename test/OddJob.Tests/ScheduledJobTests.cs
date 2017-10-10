using System;
using System.Threading;
using System.Threading.Tasks;
using OddJob.Jobs;
using OddJob.Schedules;
using Xunit;

namespace OddJob.Tests
{
    public class ScheduledJobTests
    {
        [Fact]
        public async Task WaitsForScheduleBeforeRunningWrappedJob()
        {
            var clock = Clock.DefaultClock;

            using (var cts = new CancellationTokenSource())
            {
                var job = new FakeJob(cts, clock);

                var fakeSchedule = new FakeSchedule();
                var scheduledJob = new ScheduledJob(job, fakeSchedule, NullLoggerFactory.Instance, clock);

                var timeBefore = clock.UtcNow;

                await Assert.ThrowsAsync<OperationCanceledException>(() => scheduledJob.RunAsync(cts.Token));

                Assert.True(job.RunTime.Subtract(timeBefore) >= TimeSpan.FromSeconds(2));
            }
        }

        [Fact]
        public void DisposesWrappedJob()
        {
            var job = new DisposableJob();
            var scheduledJob = new ScheduledJob(job, new FakeSchedule(), NullLoggerFactory.Instance, Clock.DefaultClock);

            scheduledJob.Dispose();

            Assert.Equal(1, job.DisposedCalls);
        }

        private class DisposableJob : IJob, IDisposable
        {
            public int DisposedCalls { get; private set; }

            public Task RunAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                this.DisposedCalls += 1;
            }
        }

        private class FakeJob : IJob
        {
            private readonly CancellationTokenSource cts;
            private readonly IClock clock;

            public FakeJob(CancellationTokenSource cts, IClock clock)
            {
                this.cts = cts;
                this.clock = clock;
            }

            public DateTime RunTime { get; private set; }

            public Task RunAsync(CancellationToken cancellationToken)
            {
                this.RunTime = this.clock.UtcNow;

                this.cts.Cancel();

                return Task.CompletedTask;
            }
        }

        private class FakeSchedule : ISchedule
        {
            public DateTime Next(DateTime @from)
            {
                return @from.AddSeconds(2);
            }
        }
    }
}
