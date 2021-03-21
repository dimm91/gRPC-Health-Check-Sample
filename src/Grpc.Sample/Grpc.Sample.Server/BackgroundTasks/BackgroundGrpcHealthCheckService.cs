using Grpc.Health.V1;
using Grpc.HealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Sample.Server.BackgroundTasks
{
    /// <summary>
    /// Background task that will periodically check the current status of the available service(s).
    /// </summary>
    public class BackgroundGrpcHealthCheckService : BackgroundService
    {
        private readonly HealthServiceImpl _healthService;
        private const string ServiceName = "GreeterService";

        public BackgroundGrpcHealthCheckService(HealthServiceImpl healthService)
        {
            _healthService = healthService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var r = new Random();

            while (!cancellationToken.IsCancellationRequested)
            {
                var randomeValue = r.Next(100);
                // You can add other Health Checks here if you are using an external API to verify if it is running.

                //Set status for Service 1
                var serviceStatus = randomeValue % 2 == 0 ? HealthCheckResponse.Types.ServingStatus.Serving :
                    HealthCheckResponse.Types.ServingStatus.NotServing;
                _healthService.SetStatus(ServiceName, serviceStatus);

                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            }
        }
    }
}
