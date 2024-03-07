using Orleans;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IScavengerObserver : IGrainObserver
    {
        Task EggFound();
    }
}
