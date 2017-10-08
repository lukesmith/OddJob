using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    /// <summary>
    /// Defines the interface for a job.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// Run the job until the <paramref name="cancellationToken"/> signals
        /// to finish.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> to
        /// signal to the job to cancel.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RunAsync(CancellationToken cancellationToken);
    }
}
