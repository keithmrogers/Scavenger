using FastEndpoints;
using FastEndpoints.Swagger;
using Scavenger.Actors;
using Scavenger.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<GameActor>();
    options.Actors.RegisterActor<GuideActor>();
    options.Actors.RegisterActor<LobbyActor>();
    options.Actors.RegisterActor<LobbyManagerActor>();
    options.Actors.RegisterActor<ScavengerActor>();
    options.Actors.RegisterActor<LeaderboardActor>();
});

builder.Services.AddSingleton(_ => new GameSettings { MinDistBetweenEggs = -25, MaxDistBetweenEggs = 25, NumberOfEggs = 10 });
builder.Services.AddSingleton<ICollisionChecker,CollisionChecker>();
builder.Services.AddDaprClient();

var app = builder.Build();

app.UseFastEndpoints()
   .UseSwaggerGen(); //add this

app.UseRouting();
app.UseCloudEvents();
app.MapActorsHandlers();

app.Run();
