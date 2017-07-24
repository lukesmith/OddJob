using System;
using System.Threading;
using System.Threading.Tasks;
using OddJob.Jobs;
using Xunit;

namespace OddJob.Tests
{
    public class JobHost_Run_Tests
    {
        [Fact]
        public async Task ForceCancellingOfJobAsync()
        {
            var job = new DelayingJob(TimeSpan.FromSeconds(5));

            var jobCompleted = await JobHost.RunAsync(job, TimeSpan.FromSeconds(1));

            Assert.False(jobCompleted, "Job should not have completed due to auto cancelling.");
            Assert.False(job.Completed, "Job should not have completed due to auto cancelling.");
        }

        [Fact]
        public async Task ForceCancellingOfForeverJobAsync()
        {
            var job = new MyForever();

            var jobCompleted = await JobHost.RunAsync(job, TimeSpan.FromSeconds(1));

            Assert.False(jobCompleted);
        }

        [Fact]
        public async Task JobCompletesWithinAllowedTimeAsync()
        {
            var job = new DelayingJob(TimeSpan.FromSeconds(1));

            var jobCompleted = await JobHost.RunAsync(job, TimeSpan.FromSeconds(2));

            Assert.True(jobCompleted);
            Assert.True(job.Completed);
        }

        [Fact]
        public async Task JobCompletesWithoutForcedDelaAsyncy()
        {
            var job = new DelayingJob(TimeSpan.FromTicks(1));

            var jobCompleted = await JobHost.RunAsync(job, CancellationToken.None);

            Assert.True(jobCompleted);
        }

        [Fact]
        public async Task JobCompletesWithoutForcedDelay_NoCancellationTokenAsync()
        {
            var job = new DelayingJob(TimeSpan.FromTicks(1));

            var jobCompleted = await JobHost.RunAsync(job);

            Assert.True(jobCompleted);
        }

        [Fact]
        public void JobCompletesWithoutForcedDelay()
        {
            var job = new DelayingJob(TimeSpan.FromTicks(1));

            var jobCompleted = JobHost.Run(job);

            Assert.True(jobCompleted);
        }

        [Fact]
        public void JobDoesNotCompleteWithForcedDelay()
        {
            var job = new DelayingJob(TimeSpan.FromSeconds(2));

            var jobCompleted = JobHost.Run(job, TimeSpan.FromSeconds(1));

            Assert.False(jobCompleted);
        }

        public class MyForever : Forever
        {
            protected override async Task DoAsync()
            {
                await Task.Delay(1000);
            }
        }

        public class DelayingJob : IJob
        {
            private readonly TimeSpan delay;

            public DelayingJob(TimeSpan delay)
            {
                this.delay = delay;
            }

            public bool Completed { get; set; }

            public async Task RunAsync(CancellationToken cancellationToken)
            {
                await Task.Delay(this.delay, cancellationToken);

                this.Completed = true;
            }
        }
    }
}
