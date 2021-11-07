using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Jobs;

namespace TaskManagement
{
    public class MonitorLoop
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;

        public MonitorLoop(IBackgroundTaskQueue taskQueue, 
            ILogger<MonitorLoop> logger, 
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void StartMonitorLoop()
        {
            _logger.LogInformation("MonitorAsync Loop is starting.");

            // Run a console user input loop in a background thread
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                // Enqueue a background work item
                Console.WriteLine("Please enter your data.");
                await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
            }
        }

        private async ValueTask BuildWorkItem(CancellationToken token)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var input = await Task.Run<string>(()=> Console.ReadLine(),token);
                    var jobs = scope.ServiceProvider.GetRequiredService<IEnumerable<IJob>>();
                    var data = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    await jobs.First(x=> x.GetType().Name == data[0]).DoWorkAsync(data[1],token);
                }
            }
            catch (OperationCanceledException ex)
            {
                // Prevent throwing if the Delay is cancelled
                _logger.LogError(ex,ex.Message);
            }
        }
    }
}