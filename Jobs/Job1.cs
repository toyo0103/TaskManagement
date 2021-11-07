using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TaskManagement.Jobs
{
    public class Job1 : IJob
    {
        private readonly Guid _jobId;
        public ILogger<Job1> _logger;

        public Job1(ILogger<Job1> logger)
        {
            _jobId = Guid.NewGuid();
            _logger = logger;
        }

        public async Task DoWorkAsync(string data, CancellationToken token)
        {
            if(token.IsCancellationRequested == false)
                _logger.LogInformation("Job1: {id} - {data}", _jobId, data);
        }
    }
}