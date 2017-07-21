using System.Threading;
using System.Threading.Tasks;

namespace OddJob.Jobs
{
    /// <summary>
    /// Represents a <see cref="IJob"/> that runs forever, or
    /// until the host process ends.
    /// </summary>
    public abstract class Forever : IJob
    {
        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    this.OnCancel();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await DoAsync();
            }
        }

        /// <summary>
        /// Implements the functionality of the job.
        /// </summary>
        /// <returns></returns>
        protected abstract Task DoAsync();

        /// <summary>
        /// Provides a method for the job to be notified 
        /// just before the job will be cancelled.
        /// </summary>
        protected virtual void OnCancel()
        {
        }
    }
}