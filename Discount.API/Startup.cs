using System.IO;
using System.Reflection;
using Common.Logging;
using Common.Logging.Correlation;
using Discount.API.Controllers;
using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.Repositories;
using Discount.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Discount.API;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateDiscountCommandHandler).GetTypeInfo().Assembly);
        services.AddScoped<ICorrelationIdGenerator, CorrelationIdGenerator>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddAutoMapper(typeof(Startup));

        // Add gRPC service
        services.AddGrpc();

        // Add controllers for REST API
        services.AddControllers();

        // Add Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Discount API",
                Version = "v1",
                Description = "Discount microservice API"
            });

            // Set the comments path for the Swagger JSON and UI
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Enable Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount API v1");
            c.RoutePrefix = "swagger";
        });

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            // Map gRPC service
            endpoints.MapGrpcService<DiscountService>();

            // Map controllers for REST API
            endpoints.MapControllers();

            // Map health check endpoint
            endpoints.MapGet("/health", async context =>
            {
                await context.Response.WriteAsync("Healthy");
            });

            // Default route
            endpoints.MapGet("/", async context =>
            {
                context.Response.Redirect("/swagger");
                await Task.CompletedTask;
            });
        });
    }
}