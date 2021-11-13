using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.Worker.Jobs.Contracts
{
    public interface IJob
    {
        Task DoWorkAsync(string data, CancellationToken token);
    }
}