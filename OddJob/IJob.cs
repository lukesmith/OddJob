using System.Threading;
using System.Threading.Tasks;

namespace OddJob
{
    public interface IJob
    {
        Task RunAsync(CancellationToken token);
    }
}