using System;
using Scavenger.Data;
using Scavenger.XForms;

[assembly: Xamarin.Forms.Dependency(typeof(ScavengerClient))]
namespace Scavenger.XForms
{
    public class ScavengerClient : IScavengerClient
    {
        public event Action OnEggFound;
        public event Action<Guid> OnLobbyReady;

        public void EggFound()
        {
            OnEggFound?.Invoke();
        }

        public void LobbyReady(Guid scavengerId)
        {
            OnLobbyReady?.Invoke(scavengerId);
        }
    }
}
