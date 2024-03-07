using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Scavenger.XForms.Droid.Services
{
    class ClientTerminal
    {
        Socket _socketClient;
        private SocketListener _listener;

        public event Action<Socket, byte[]> MessageRecived;
        public event Action<Socket> Connected;
        public event Action<Socket> Disconnected;

        public void Connect(IPAddress remoteIPAddress, int alPort)
        {
            _socketClient = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, alPort);
            try
            {
                _socketClient.Connect(remoteEndPoint);
            }
            catch (SocketException ex)
            {
                Debug.Write(ex);
                throw;
            }

            OnServerConnection();
        }

        public void SendMessage(byte[] buffer)
        {
            if (_socketClient == null)
            {
                return;
            }
            _socketClient.Send(buffer);

        }

        public void StartListening()
        {
            if (_socketClient == null)
            {
                return;
            }

            if (_listener != null)
            {
                return;
            }

            _listener = new SocketListener();
            _listener.Disconnected += OnServerConnectionDropped;
            _listener.MessageReceived += OnMessageRecvied;

            _listener.StartReceiving(_socketClient);
        }

        public void Close()
        {
            if (_socketClient == null)
            {
                return;
            }

            _listener?.StopListening();

            _socketClient.Close();
            _listener = null;
            _socketClient = null;
        }

        private void OnServerConnection()
        {
            Connected?.Invoke(_socketClient);
        }

        private void OnMessageRecvied(Socket socket, byte[] buffer)
        {
            MessageRecived?.Invoke(socket, buffer);
        }

        private void OnServerConnectionDropped(Socket socket)
        {
            Close();
            RaiseServerDisconnected(socket);
        }

        private void RaiseServerDisconnected(Socket socket)
        {
            Disconnected?.Invoke(socket);
        }
    }
}