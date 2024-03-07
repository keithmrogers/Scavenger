
using System.Collections.Generic;
using System;
namespace Scavenger.Core;

public class Game(Guid gameId, GameSettings settings) : Entity
{
    private IList<Egg> listOfEggs = new List<Egg>();
    private Position? lastEggPosition;
    private DateTime? lastEggFoundTime;
    private readonly GameSettings settings = settings;

    public Game(string gameId, GameSettings settings) : this(Guid.Parse(gameId), settings)
    {
    }

    public Guid ScavengerId { get; private set; }
    public Guid GuideId { get; private set; }
    public Guid GameId { get; } = gameId;

    // Use this for initialization
    public void Start(Guid scavengerId, Guid guideId)
    {
        ScavengerId = scavengerId;
        GuideId = guideId;
        lastEggPosition = null;
        lastEggFoundTime = null;
        listOfEggs = new List<Egg>();

        PlaceEggs();

        DomainEvents.Add(new GameStartedEvent { ScavengerId = ScavengerId, GameId = GameId, GuideId = GuideId });
    }

    public void ResetGame()
    {
        listOfEggs.Clear();
        PlaceEggs();
    }

    public void CheckFoundEgg(Scavenger scavenger, ICollisionChecker collisionChecker)
    {
        var foundEgg = collisionChecker.CheckCollision(scavenger, listOfEggs);

        if (foundEgg != null)
        {
            FoundEgg(scavenger, foundEgg);
        }
    }

    private void FoundEgg(Scavenger scavenger, Egg foundEgg)
    {
        listOfEggs.Remove(foundEgg);

        var eggFoundTime = DateTime.Now;
        var result = new EggFoundResult
        {
            Distance = GetDistance(lastEggPosition ?? Scavenger.StartPosition, scavenger.Position),
            TimeSeconds = (eggFoundTime - (lastEggFoundTime ?? scavenger.StartTime)).Seconds
        };
        AddDomainEvent(new EggFoundEvent { GameId = this.GameId });

        lastEggPosition = foundEgg.Position;
        lastEggFoundTime = eggFoundTime;
    }

    private static double GetDistance(Position lastEggPosition, Position position)
    {
        var a = (double)(position.X - lastEggPosition.X);
        var b = (double)(position.Y - lastEggPosition.Y);

        return Math.Sqrt(a * a + b * b);
    }

    private void PlaceEggs()
    {
        int numberForEachQuadrant = settings.NumberOfEggs / 4;
        int numberInCurrentQuadrant = 0;
        int currentQuadrant = 0;

        for (int i = 0; i < settings.NumberOfEggs; i++)
        {
            var newEggPosition = GetRandomEggLocation(currentQuadrant);
            listOfEggs.Add(new Egg(newEggPosition));

            numberInCurrentQuadrant++;

            if (numberInCurrentQuadrant == numberForEachQuadrant)
            {
                currentQuadrant++;
                numberInCurrentQuadrant = 0;
            }
        }
    }

    #region Egg Location
    public Position GetRandomEggLocation(int quadrant)
    {
        var x = GetRandomXValue(quadrant);
        var y = GetRandomYValue(quadrant);

        int tries = 0;
        //minimum distance from start area
        while (x <= settings.MaxDistBetweenEggs && x >= settings.MinDistBetweenEggs &&
            y <= settings.MaxDistBetweenEggs && y >= settings.MinDistBetweenEggs || tries > 5)
        {
            x = GetRandomXValue(quadrant);
            y = GetRandomYValue(quadrant);

            //give up after a while, don't get stuck, not likely... but...
            tries++;
        }

        tries = 0;
        //minimum distance from each other
        while (IsTooCloseToEgg(x, y) || tries > 5)
        {
            x = GetRandomXValue(quadrant);
            y = GetRandomYValue(quadrant);

            tries++;
        }

        return new Position(x, y);
    }

    private int GetRandomXValue(int quadrant)
    {
        if (quadrant == 0)
        {
            return Random.Shared.Next(-250, 0);
        }
        else if (quadrant == 1)
        {
            return Random.Shared.Next(0, 250);
        }
        else if (quadrant == 2)
        {
            return Random.Shared.Next(-250, 0);
        }
        else
        {
            return Random.Shared.Next(0, 250);
        }
    }

    private int GetRandomYValue(int quadrant)
    {
        if (quadrant == 0 || quadrant == 1)
        {
            return Random.Shared.Next(0, 250);
        }
        else
        {
            return Random.Shared.Next(-250, 0);
        }
    }

    private bool IsTooCloseToEgg(int xValue, int yValue)
    {
        foreach (var item in listOfEggs)
        {
            var eggDeltaX = item.Position.X - xValue;
            var eggDeltaY = item.Position.Y - yValue;

            if (eggDeltaX <= settings.MaxDistBetweenEggs && eggDeltaX >= settings.MinDistBetweenEggs &&
            eggDeltaY <= settings.MaxDistBetweenEggs && eggDeltaY >= settings.MinDistBetweenEggs)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}