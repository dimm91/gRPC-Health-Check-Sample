using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grpc.Sample.Server.Services
{
    public class GrpcHealthCheckService : Health.V1.Health.HealthBase
    {
        private const string ServiceName = "GreeterService";

        /// <summary>
        /// Unary health check request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            var r = new Random();
            var randomeValue = r.Next(100);
            var resp = new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.Unknown
            };

            //Check the service it was requested
            if (request.Service.Equals(ServiceName, StringComparison.InvariantCultureIgnoreCase))
            {
                resp.Status = randomeValue % 2 == 0 ? HealthCheckResponse.Types.ServingStatus.Serving :
                    HealthCheckResponse.Types.ServingStatus.NotServing;
            }
            return resp;
        }

        /// <summary>
        /// Stream health check request if the status change it will notify the connected clients
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task Watch(HealthCheckRequest request, IServerStreamWriter<HealthCheckResponse> responseStream, ServerCallContext context)
        {
            var r = new Random();
            var resp = new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.Unknown
            };

            while (true)
            {
                var randomeValue = r.Next(100);

                //Check the service it was requested
                if (request.Service.Equals(ServiceName, StringComparison.InvariantCultureIgnoreCase))
                {
                    resp.Status = randomeValue % 2 == 0 ? HealthCheckResponse.Types.ServingStatus.Serving :
                    HealthCheckResponse.Types.ServingStatus.NotServing;
                }

                await responseStream.WriteAsync(resp);

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}
