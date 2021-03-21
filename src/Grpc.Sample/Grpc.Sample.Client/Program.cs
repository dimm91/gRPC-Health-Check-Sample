using Grpc.Health.V1;
using Grpc.Health;
using Grpc.Net.Client;
using GrpcGreeterClient;
using System;
using System.Threading.Tasks;
using Grpc.Core;

namespace Grpc.Sample.Client
{
    class Program
    {
        private const string ServerAddress = "https://localhost:5001";
        private const string ServiceName = "GreeterService";
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    using var channel = GrpcChannel.ForAddress(ServerAddress);

                    var healthCheckClient = new Health.V1.Health.HealthClient(channel);
                    var healthCheckResponse = healthCheckClient.Check(new HealthCheckRequest { Service = ServiceName });
                    Console.WriteLine($"Service: '{ServiceName}' has a health status: {healthCheckResponse.Status}");

                    if (healthCheckResponse.Status == HealthCheckResponse.Types.ServingStatus.Serving)
                    {
                        var client = new Greeter.GreeterClient(channel);
                        var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
                        Console.WriteLine("Greeting: " + reply.Message);
                    }
                    else
                    {
                        // Notify - Log - Workaround when the service is not available
                    }

                    // If there is a change on the current Health status of the server we will receive the notification
                    //var watchStream = healthCheckClient.Watch(new HealthCheckRequest() { Service = ServiceName });
                    //while (await watchStream.ResponseStream.MoveNext())
                    //{
                    //    Console.WriteLine($"[{DateTime.UtcNow}] - Received: " + watchStream.ResponseStream.Current.Status.ToString());
                    //}

                    await Task.Delay(1000);
                }
                catch (RpcException ex)
                {
                    //Log exception
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}
