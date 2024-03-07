using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Scavenger.Api;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddAuthentication().AddDapr();
builder.Services.AddAuthorization(options =>
{
    options.AddDapr();
});

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.ShortSchemaNames = true;
});

builder.Services.AddActors((options) => { });
builder.Services.AddSingleton<IEventChannelManager, EventChannelManager>();

if (!builder.Environment.IsDevelopment())
{
    // Add OpenTelemetry and configure it to use Azure Monitor.
    builder.Services.AddOpenTelemetry().UseAzureMonitor();
}

var app = builder.Build();

app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

app.MapSubscribeHandler();
app.UseFastEndpoints()
   .UseSwaggerGen(); //add this

app.Run();