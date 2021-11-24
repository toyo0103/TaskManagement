using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.SDK.Extensions;
using TaskManagement.Worker.Jobs;

namespace TaskManagement.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostBuilder =  CreateHostBuilder(args).Build();
            var monitorLoop = hostBuilder.Services.GetRequiredService<IPC>();
            monitorLoop.StartMonitorLoop();
            hostBuilder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTaskManagement()
                            .Register<Job1>()
                            .Register<Job2>();
                    services.AddSingleton<MonitorLoop>();
                    services.AddSingleton<IPC>();
                });
    }
}
