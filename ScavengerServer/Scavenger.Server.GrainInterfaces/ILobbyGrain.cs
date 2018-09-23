using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Scavenger.Server.Domain;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {

        Task ScavengerJoin(ILobbyObserver observer, IScavengerObserver scavengerObserver);
        Task GuideJoin(ILobbyObserver lobbyObserver, IGuideObserver guideObserver);

        //Task Subscribe(ILobbyObserver observer);
        //Task Unsubscribe(ILobbyObserver observer);
    }
}
