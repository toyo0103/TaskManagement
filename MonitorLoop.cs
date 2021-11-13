using System;
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
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;

        public MonitorLoop(IBackgroundTaskQueue taskQueue, 
            ILogger<MonitorLoop> logger, 
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider)
        {
            _taskQueue = taskQueue;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _serviceProvider = serviceProvider;
            _cancellationToken = _applicationLifetime.ApplicationStopping;
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
                var input = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(input) == false)
                {
                    InputData = input;
                    await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
                }
                else
                {
                    _applicationLifetime.StopApplication();
                }
            }
        }
        private string InputData = string.Empty;
        private async ValueTask BuildWorkItem(CancellationToken token)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var data = InputData.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    await ((IJob)scope.ServiceProvider.GetRequiredService(Type.GetType(data[0])))
                        .DoWorkAsync(data[1],token);
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