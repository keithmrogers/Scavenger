using FastEndpoints;
using FastEndpoints.Swagger;
using Orleans.Configuration;
using Orleans.Hosting;
using System.Net;
using System;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.ShortSchemaNames = true;
});

builder.Host.UseOrleans((context, siloBuilder) =>
    {
        if (builder.Environment.IsDevelopment())
        {
            siloBuilder
                .UseLocalhostClustering();
        }
        else
        {
            var siloPort = 11111;
            var gatewayPort = 30000;

            siloBuilder.ConfigureEndpoints(siloPort, gatewayPort);

            var connectionString = context.Configuration["ORLEANS_AZURE_STORAGE_CONNECTION_STRING"];

            siloBuilder.Configure<ClusterOptions>(
            options =>
            {
                options.ClusterId = context.Configuration["ORLEANS_CLUSTER_ID"];
                options.ServiceId = nameof(Scavenger.Server);
            })
                .UseAzureStorageClustering(
                    options =>
                    {
                        options.ConfigureTableServiceClient(connectionString);
                        options.TableName = $"{context.Configuration["ORLEANS_CLUSTER_ID"]}Clustering";
                    })
                .AddAzureTableGrainStorage("scavenger",
                    options =>
                    {
                        options.ConfigureTableServiceClient(connectionString);
                        options.TableName = $"{context.Configuration["ORLEANS_CLUSTER_ID"]}Persistence";
                    });
        }
    });

    var app = builder.Build();

    app.UseFastEndpoints()
       .UseSwaggerGen(); //add this

    app.Run();