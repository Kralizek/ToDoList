using System;
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
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HostEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddSingleton<IStorage, InMemoryStorage>();

            // services.AddSingleton<IStorage, DynamoDBTableStorage>();

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

                    options.Enrich = (activity, _, _) =>
                    {
                        activity.SetTag("environment", HostEnvironment.EnvironmentName);

                        activity.SetTag("project", "ToDoList");
                    };
                });

                builder.AddGrpcClientInstrumentation();

                builder.AddHttpClientInstrumentation(options => options.RecordException = true);

                builder.AddConsoleExporter();

                builder.AddOtlpExporter(options => options.Endpoint = Configuration.GetServiceUri("otel"));
            });
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
                endpoints.MapGrpcService<ToDoService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}
