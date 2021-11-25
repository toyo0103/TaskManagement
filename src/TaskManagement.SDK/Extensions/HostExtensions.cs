using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.SDK.Job.Contracts;
using TaskManagement.SDK.Mode;

namespace TaskManagement.SDK.Extensions
{
    public static class HostBuilderExtensions
    {
        public static void RunConsumer(this IHost host)
        {
            var mode = host.Services.GetRequiredService<IMode>();
            mode.StartCosuming();
            host.Run();
        }
    }
}
