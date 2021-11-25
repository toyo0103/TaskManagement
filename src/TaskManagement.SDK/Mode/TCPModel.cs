using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetMQ;
using NetMQ.Sockets;
using TaskManagement.SDK.Extensions;
using TaskManagement.SDK.Job.Contracts;
using TaskManagement.SDK.Model;

namespace TaskManagement.SDK.Mode
{
    //https://github.com/zeromq/netmq
    internal class TCPModel : IMode
    {
        private readonly string _port;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationToken _cancellationToken;

        public TCPModel(
            string port,
            IBackgroundTaskQueue taskQueue, 
            ILogger<TCPModel> logger, 
            IHostApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider)
        {
            _port = port;
            _taskQueue = taskQueue;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _serviceProvider = serviceProvider;
            _cancellationToken = _applicationLifetime.ApplicationStopping;
        }

        public void StartCosuming()
        {
            _logger.LogInformation("Tcp mode is starting.");
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            using (var server = new ResponseSocket($"@tcp://localhost:{_port}")) // bind
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    // Receive the message from the server socket
                    var input = server.ReceiveFrameString();
                    Console.WriteLine("From Client: {0}", input);
                    var deserializeResult = await JsonSerializerExtensions.TryDeserializeAsync<TaskModel>(input);
                    if(deserializeResult.result == false) 
                    {
                        Console.WriteLine($"Can't deserialize to task model. Input data is {input}");
                        server.SendFrame("exit");
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

                    // Send a response back from the server
                    server.SendFrame("exit");
                }   
            }
        }
    }
}