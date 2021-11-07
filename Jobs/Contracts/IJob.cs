using System.Threading;
using System.Threading.Tasks;

namespace TaskManagement.Jobs
{
    public interface IJob
    {
        Task DoWorkAsync(string data, CancellationToken token);
    }
}