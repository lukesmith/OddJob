using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    /// <summary>
    /// Defines an interface for something that hosts one or more <see cref="IJob"/>.
    /// </summary>
    public interface IJobHost : IDisposable
    {
        /// <summary>
        /// Starts the jobs managed by the host.
        /// </summary>
        /// <param name="cts">A <see cref="CancellationTokenSource"/> that will cancel the jobs.</param>
        Task StartAsync(CancellationTokenSource cts);

        /// <summary>
        /// Returns the <see cref="IJob"/> the host is managing.
        /// </summary>
        IEnumerable<IJob> Jobs { get; }
    }
}