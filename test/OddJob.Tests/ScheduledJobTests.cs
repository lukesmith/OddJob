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

                var fakeSchedule = new FakeSchedule(2);
                var scheduledJob = new ScheduledJob<FakeJob>(() => job, fakeSchedule, NullLoggerFactory.Instance, clock);

                var timeBefore = clock.UtcNow;

                await Assert.ThrowsAsync<OperationCanceledException>(() => scheduledJob.RunAsync(cts.Token));

                Assert.True(job.RunTime.Subtract(timeBefore) >= TimeSpan.FromSeconds(2));
            }
        }

        [Fact]
        public async Task DisposesWrappedJob()
        {
            var job = new DisposableJob();
            var scheduledJob = new ScheduledJob<DisposableJob>(() => job, new FakeSchedule(2), NullLoggerFactory.Instance, Clock.DefaultClock);

            using (var cts = new CancellationTokenSource())
            {
                await JobHost.RunAsync(scheduledJob, TimeSpan.FromSeconds(3), cts.Token);
            }

            Assert.True(job.DisposedCalls > 0);
        }

        [Fact]
        public async Task SwallowsExceptionsInInnerJob()
        {
            var clock = Clock.DefaultClock;

            using (var cts = new CancellationTokenSource())
            {
                var job = new FakeFailingJob();

                var fakeSchedule = new FakeSchedule(1);
                var scheduledJob = new ScheduledJob<FakeFailingJob>(() => job, fakeSchedule, NullLoggerFactory.Instance, clock);

                await JobHost.RunAsync(scheduledJob, TimeSpan.FromSeconds(3), cts.Token);

                Assert.True(job.RunCallCount > 1);
            }
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

        private class FakeFailingJob : IJob
        {
            public int RunCallCount { get; private set; }

            public Task RunAsync(CancellationToken cancellationToken)
            {
                this.RunCallCount++;
                throw new Exception("Failure within job");
            }
        }

        private class FakeSchedule : ISchedule
        {
            private readonly int secondsDelay;

            public FakeSchedule(int secondsDelay)
            {
                this.secondsDelay = secondsDelay;
            }

            public DateTime Next(DateTime @from)
            {
                return @from.AddSeconds(this.secondsDelay);
            }
        }
    }
}
