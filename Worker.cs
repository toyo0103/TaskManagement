using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TaskManagement
{
    public class Worker : IHostedService
    {
        private readonly IHostApplicationLifetime _appLifetime;

        public Worker(IHostApplicationLifetime appLifetime)
        {
            this._appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StartAsync");
            _appLifetime.ApplicationStarted.Register(OnStarted);
            // _appLifetime.ApplicationStopping.Register(OnStopping);
            // _appLifetime.ApplicationStopped.Register(OnStopped);
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            Console.WriteLine("OnStarted has been called.");

            // Perform post-startup activities here
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("StopAsync");
            return Task.CompletedTask;
        }
    }
}