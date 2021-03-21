using Grpc.Core;
using Grpc.Health.V1;
using Grpc.HealthCheck;
using Grpc.Sample.Server.BackgroundTasks;
using Grpc.Sample.Server.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Sample.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddSingleton<HealthServiceImpl>();
            services.AddHostedService<BackgroundGrpcHealthCheckService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //Use only one Health check, if you are using the default implementation 'HealthServiceImpl' enable the background task declared before

                /// Override the default 'HealthServiceImpl' with the implementation defined in: <see cref="GrpcHealthCheckService"/>
                //endpoints.MapGrpcService<GrpcHealthCheckService>();

                // Use the default implementation to check the current health of the app
                /// The background task is setting the status for 'HealthServiceImpl' <see cref="BackgroundGrpcHealthCheckService"/>
                endpoints.MapGrpcService<HealthServiceImpl>();

                endpoints.MapGrpcService<GreeterService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
