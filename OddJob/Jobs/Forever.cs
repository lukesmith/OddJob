using System.Threading;
using System.Threading.Tasks;

namespace OddJob.Jobs
{
    public abstract class Forever : IJob
    {
        public async Task RunAsync(CancellationToken token)
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();

                await DoAsync();
            }
        }

        protected abstract Task DoAsync();
    }
}