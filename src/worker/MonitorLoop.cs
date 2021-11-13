using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagement.Worker.Extensions;
using TaskManagement.Worker.Jobs.Contracts;

namespace TaskManagement.Worker
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
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var input = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(input) == false)
                {
                    var deserializeResult = await JsonSerializerExtensions.TryDeserializeAsync<TaskModel>(input);
                    if(deserializeResult.result == false) 
                    {
                        Console.WriteLine($"Can't deserialize to task model. Input data is {input}");
                        continue;
                    }
                    
                    await _taskQueue.QueueBackgroundWorkItemAsync(async (token) =>
                    {
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var jobs = scope.ServiceProvider.GetRequiredService<IEnumerable<IJob>>();
                                var targetJob = jobs.First(x=> x.GetType().Name == deserializeResult.data.JobName);
                                await targetJob.DoWorkAsync(deserializeResult.data.Data,token);
                            }
                        }
                        catch (OperationCanceledException ex)
                        {
                            // Prevent throwing if the Delay is cancelled
                            _logger.LogError(ex,ex.Message);
                        }
                    });
                }
                else
                {
                    _applicationLifetime.StopApplication();
                }
            }
        }
    }
}