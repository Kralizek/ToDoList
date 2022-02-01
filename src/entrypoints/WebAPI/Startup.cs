using System;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Resources;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Service;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            HostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Sdk.SetDefaultTextMapPropagator(new AWSXRayPropagator());
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment HostEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList", Version = "v1" });
            });

            services.AddTransient<Services.IToDoClient>(sp => sp.GetRequiredService<ToDo.ToDoClient>());

            services.AddGrpcClient<ToDo.ToDoClient>(o => o.Address = Configuration.GetServiceUri("service"));

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddXRayTraceId();

                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                                                          .AddService("WebAPI")
                                                          .AddDetector(new AWSECSResourceDetector())
                                                          .AddTelemetrySdk()
                                                          .AddEnvironmentVariableDetector());


                builder.AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;

                    options.Enrich = (activity, eventName, rawObject) =>
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

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoList v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
