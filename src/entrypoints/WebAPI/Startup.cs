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
using Microsoft.OpenApi.Models;
using Service;

using Grpc.Net.Client.Configuration;
using RetryPolicy = Grpc.Net.Client.Configuration.RetryPolicy;
using Grpc.Core;

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoList", Version = "v1" });
            });

            services.AddTransient<HttpClientXRayTracingHandler>();

            services.AddTransient<Services.IToDoClient>(sp => sp.GetRequiredService<ToDo.ToDoClient>());

            services.AddGrpcClient<ToDo.ToDoClient>(o =>
            {
                o.Address = Configuration.GetServiceUri("service");
            }).AddHttpMessageHandler<HttpClientXRayTracingHandler>()
            .SetFallbackRetryPolicy(new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromSeconds(1),
                MaxBackoff = TimeSpan.FromSeconds(5),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
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
