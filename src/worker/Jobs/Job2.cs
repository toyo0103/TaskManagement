using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManagement.Worker.Jobs.Contracts;

namespace TaskManagement.Worker.Jobs
{
    public class Job2 : IJob
    {
        private readonly Guid _jobId;
        public ILogger _logger;

        public Job2(ILogger<Job2> logger)
        {
            _jobId = Guid.NewGuid();
            _logger = logger;
        }

        public async Task DoWorkAsync(string data, CancellationToken token)
        {
            if(token.IsCancellationRequested == false)
                _logger.LogInformation("Job2: {id} - {data}", _jobId, data);
        }
    }
}