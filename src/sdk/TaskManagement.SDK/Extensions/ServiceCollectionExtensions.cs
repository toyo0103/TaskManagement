using Microsoft.Extensions.DependencyInjection;
using TaskManagement.SDK.Job.Contracts;

namespace TaskManagement.SDK.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskManagement(this IServiceCollection services)
        {
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                //if (!int.TryParse(hostContext.Configuration["QueueCapacity"], out var queueCapacity))
                var queueCapacity = 1;
                return new BackgroundTaskQueue(queueCapacity);
            });
            return services;
        }

        public static IServiceCollection Register<T>(this IServiceCollection services) 
            where T : class, IJob 
        {
            services.AddScoped<IJob,T>();
            return services;
        }
    }
}
