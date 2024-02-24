
using Dapr.Client;
using Dapr.Actors.Runtime;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.ActorService;

public class GameActor(ActorHost host, GameSettings gameSettings, ICollisionChecker collisionChecker, DaprClient client) : Actor(host), IGameActor
{
    private Game? game = null;
    private readonly ICollisionChecker collisionChecker = collisionChecker;
    private readonly DaprClient client = client;

    protected override Task OnActivateAsync()
    {
        game = new Game(this.Id.GetId(), gameSettings);
        return Task.CompletedTask;
    }

    public async Task Start(Guid scavengerId, Guid guideId)
    {

        Console.WriteLine($"Starting Game {this.Id}");
        game!.Start(scavengerId, guideId);

        var scavengerActor = this.ProxyFactory.CreateActorProxy<IScavengerActor>(scavengerId.ToActorId(), nameof(ScavengerActor));
        await scavengerActor!.Start(game.GameId);

        var guideActor = this.ProxyFactory.CreateActorProxy<IGuideActor>(guideId.ToActorId(), nameof(GuideActor));
        await guideActor.SetGameId(game.GameId);

        await DispatchEvents(game);

        Console.WriteLine($"Game {this.Id} Started!");
    }

    public async Task CheckFoundEgg()
    {
        if (game?.ScavengerId == null) throw new ArgumentException("ScavengerId has not been set yet");

        var scavenger = this.ProxyFactory.CreateActorProxy<IScavengerActor>(game.ScavengerId.ToActorId(), nameof(ScavengerActor));
        game.CheckFoundEgg(await scavenger.GetScavenger(), collisionChecker);

        await DispatchEvents(game);
    }

    private async Task DispatchEvents(Entity entity)
    {
        try
        {
            await client.PublishDomainEvents("games", entity);
            entity.DomainEvents.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task<Guid> GetScavengerId()
    {
        return await Task.FromResult(game!.ScavengerId);
    }

    public async Task<Guid> GetGuideId()
    {
        return await Task.FromResult(game!.GuideId);
    }
}