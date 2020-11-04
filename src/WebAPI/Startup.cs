using System;
using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Amazon.XRay.Recorder.Handlers.System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AWSXRayRecorder.InitializeInstance(configuration);
            AWSXRayRecorder.RegisterLogger(Amazon.LoggingOptions.Console);
            AWSSDKHandler.RegisterXRayForAllServices();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddTransient<HttpClientXRayTracingHandler>();

            services.AddGrpcClient<ToDo.ToDoClient>(o =>
            {
                o.Address = Configuration.GetServiceUri("service");
            }).AddHttpMessageHandler<HttpClientXRayTracingHandler>();

            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseXRay("ToDo:WebAPI");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
