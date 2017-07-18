using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    public interface IJobHost : IDisposable
    {
        Task StartAsync(CancellationTokenSource token);

        IEnumerable<IJob> Jobs { get; }
    }
}