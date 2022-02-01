using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// using Amazon.XRay.Recorder.Core;
// using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Resources;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            // AWSXRayRecorder.InitializeInstance(configuration);
            // AWSXRayRecorder.RegisterLogger(Amazon.LoggingOptions.Console);
            // AWSSDKHandler.RegisterXRayForAllServices();

            Configuration = configuration;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddSingleton<IStorage, InMemoryStorage>();

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddXRayTraceId();

                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                                                            .AddService("Service")
                                                            .AddDetector(new AWSECSResourceDetector())
                                                            .AddTelemetrySdk()
                                                            .AddEnvironmentVariableDetector());

                builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                });

                builder.AddGrpcClientInstrumentation(options =>
                {
                    // needed because of AddHttpClientInstrumentation
                    options.SuppressDownstreamInstrumentation = true;
                });

                builder.AddHttpClientInstrumentation(options =>
                {
                    options.RecordException = true;
                });

                builder.AddConsoleExporter();

                builder.AddOtlpExporter(options => 
                {
                    // options.Endpoint = new Uri(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT"));

                    options.Endpoint = Configuration.GetServiceUri("otel");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseXRay("ToDo:Service");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<ToDoService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
