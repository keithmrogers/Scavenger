using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.IO;

namespace Scavenger.Server
{
    public interface IGuideClient
    {
        void LobbyReady(Guid clientId);
        void ScavengerMoved(Position position);
        void ScavengerChangedDirection(double direction);
    }
    public class TestGuideClient : IGuideClient
    {
        public event Action<Guid> OnLobbyReady;
        public event Action<Position> OnScavengerMoved;
        public event Action<double> OnScavengerChangedDirection;

        public void LobbyReady(Guid scavengerId)
        {
            if (OnLobbyReady != null)
            {
                OnLobbyReady.Invoke(scavengerId);
            }
        }
        public void ScavengerMoved(Position position)
        {
            if (OnScavengerMoved != null)
            {
                OnScavengerMoved.Invoke(position);
            }
        }
        public void ScavengerChangedDirection(double direction)
        {
            if (OnScavengerChangedDirection != null)
            {
                OnScavengerChangedDirection.Invoke(direction);
            }
        }
    }
}