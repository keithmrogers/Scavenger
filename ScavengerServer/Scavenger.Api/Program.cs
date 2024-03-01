using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Scavenger.Api;

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

var app = builder.Build();

app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

app.MapSubscribeHandler();
app.UseFastEndpoints()
   .UseSwaggerGen(); //add this

app.Run();