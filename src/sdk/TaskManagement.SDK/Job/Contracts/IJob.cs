using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.SDK.Job.Contracts
{
    public interface IJob
    {
        Task DoWorkAsync(string data, CancellationToken token);
    }
}