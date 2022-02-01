using System;
using Kralizek.Extensions.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Resources;
using OpenTelemetry.Contrib.Extensions.AWSXRay.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Web
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
            services.AddRazorPages();

            services.AddHttpRestClient("backend", builder => 
            {
                builder.ConfigureHttpClient(http => http.BaseAddress = Configuration.GetServiceUri("webapi"));
            });

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddXRayTraceId();

                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                                                          .AddService("Web")
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
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
