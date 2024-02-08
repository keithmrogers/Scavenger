using FastEndpoints;
using FastEndpoints.Swagger;
using Orleans.Hosting;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.ShortSchemaNames = true;
});

if (builder.Environment.IsDevelopment())
{
    builder.Host.UseOrleans((_, builder) =>
    {
        builder
            .UseLocalhostClustering();
    });
}

var app = builder.Build();

app.UseFastEndpoints()
   .UseSwaggerGen(); //add this

app.Run();