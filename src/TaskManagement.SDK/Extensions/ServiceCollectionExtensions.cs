using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.SDK.Job.Contracts;
using TaskManagement.SDK.Mode;

namespace TaskManagement.SDK.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskManagement(this IServiceCollection services, string[] args)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                //if (!int.TryParse(hostContext.Configuration["QueueCapacity"], out var queueCapacity))
                var queueCapacity = 1;
                return new BackgroundTaskQueue(queueCapacity);
            });

            var result = ParseOpts(args);

            switch (result.mode)
            {
                case ModeEnum.Tcp:
                    services.AddSingleton<IMode>(sp=>
                    {
                        return new TCPModel(
                            result.port,
                            taskQueue: sp.GetRequiredService<IBackgroundTaskQueue>(),
                            logger: sp.GetRequiredService<ILogger<TCPModel>>(),
                            applicationLifetime: sp.GetRequiredService<IHostApplicationLifetime>(),
                            sp
                        );
                    });
                    Console.WriteLine("using tcp mode to start ....");
                    break;
                default:
                    services.AddSingleton<IMode, TerminalMode>();
                    Console.WriteLine("using terminal mode to start ....");
                    break;
            }
            return services;
        }

        private static (ModeEnum mode, string port) ParseOpts(string[] args)
        {
            if(args.Length == 0 || Array.IndexOf(args, "--terminal") >= 0) 
                return (ModeEnum.Terminal , string.Empty);
            
            var tcpIndex = Array.IndexOf(args, "--tcp");
            if(tcpIndex >=0)
                return (ModeEnum.Tcp , args[tcpIndex+1]);

            throw new ArgumentException("Unkown mode.");
        }


        public static IServiceCollection Register<T>(this IServiceCollection services) 
            where T : class, IJob 
        {
            services.AddScoped<IJob,T>();
            return services;
        }
    }
}
