using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskManagement.SDK.Extensions;
using TaskManagement.SDK.Mode;
using TaskManagement.Worker.Jobs;

namespace TaskManagement.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var host =  CreateHostBuilder(args).Build();
            host.RunConsumer();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // init task management
                    services.AddTaskManagement(args)
                            .Register<Job1>()  // register your job
                            .Register<Job2>();
                });
    }
}
