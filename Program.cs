using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.Jobs;

namespace TaskManagement
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostBuilder =  CreateHostBuilder(args).Build();
            var monitorLoop = hostBuilder.Services.GetRequiredService<MonitorLoop>();
            monitorLoop.StartMonitorLoop();
            hostBuilder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<Job1>();
                    services.AddTransient<Job2>();
                    services.AddSingleton<MonitorLoop>();
                    services.AddHostedService<QueuedHostedService>();
                    services.AddSingleton<IBackgroundTaskQueue>(ctx =>
                    {
                        if (!int.TryParse(hostContext.Configuration["QueueCapacity"], out var queueCapacity))
                            queueCapacity = 1;
                        return new BackgroundTaskQueue(queueCapacity);
                    });
                });

    }
}
