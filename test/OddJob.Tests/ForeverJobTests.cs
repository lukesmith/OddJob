using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OddJob.Tests
{
    public class ForeverJobTests
    {
        [Fact]
        public async Task CancellingForeverJobCallsOnCancel()
        {
            var job = new FakeForever();

            using (var cts = new CancellationTokenSource())
            {
                cts.Cancel();

                await Assert.ThrowsAsync<OperationCanceledException>(() => job.RunAsync(cts.Token));
            }

            Assert.True(job.OnCancelCalled);
        }

        private class FakeForever : Jobs.Forever
        {
            public bool OnCancelCalled { get; private set; }

            protected override async Task DoAsync()
            {
                await Task.Delay(1);
            }

            protected override void OnCancel()
            {
                this.OnCancelCalled = true;
                base.OnCancel();
            }
        }
    }
}
