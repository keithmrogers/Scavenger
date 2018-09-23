using System.Net.Sockets;
using System;
using System.Net;
using UnityEngine;

namespace Scavenger.Server
{
    class ClientTerminal
    {
        Socket _socketClient;
        private SocketListener _listener;

        public event Action<Socket, byte[]> MessageRecived;
        public event Action<Socket> Connected;
        public event Action<Socket> Disconncted;

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
                Debug.Log(ex);
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

            if (_listener != null)
            {
                _listener.StopListening();
            }

            _socketClient.Close();
            _listener = null;
            _socketClient = null;
        }

        private void OnServerConnection()
        {
            if (Connected != null)
            {
                Connected.Invoke(_socketClient);
            }
        }

        private void OnMessageRecvied(Socket socket, byte[] buffer)
        {
            if (MessageRecived != null)
            {
                MessageRecived.Invoke(socket, buffer);
            }
        }

        private void OnServerConnectionDropped(Socket socket)
        {
            Close();
            RaiseServerDisconnected(socket);
        }

        private void RaiseServerDisconnected(Socket socket)
        {
            if (Disconncted != null)
            {
                Disconncted.Invoke(socket);
            }
        }
    }

    //public class ClientTerminal
    //{
    //    Socket _socketClient;
    //    private SocketListener _listener;

    //    public event Action<Socket, byte[]> MessageRecived;
    //    public event Action<Socket> Connected;
    //    public event Action<Socket> Disconncted;

    //    public void Connect(IPAddress remoteIPAddress, int alPort)
    //    {
    //        _socketClient = new Socket(AddressFamily.InterNetwork,
    //            SocketType.Stream, ProtocolType.Tcp);

    //        IPEndPoint remoteEndPoint = new IPEndPoint(remoteIPAddress, alPort);
    //        try
    //        {
    //            _socketClient.Connect(remoteEndPoint);
    //        }
    //        catch (SocketException ex)
    //        {
    //            //Debug.Write(ex);
    //            throw;
    //        }

    //        OnServerConnection();
    //    }

    //    public void SendMessage(byte[] buffer)
    //    {
    //        if (_socketClient == null)
    //        {
    //            return;
    //        }
    //        _socketClient.Send(buffer);

    //    }

    //    public void StartListening()
    //    {
    //        if (_socketClient == null)
    //        {
    //            return;
    //        }

    //        if (_listener != null)
    //        {
    //            return;
    //        }

    //        _listener = new SocketListener();
    //        _listener.Disconnected += OnServerConnectionDropped;
    //        _listener.MessageReceived += OnMessageRecvied;

    //        _listener.StartReceiving(_socketClient);
    //    }

    //    public void Close()
    //    {
    //        if (_socketClient == null)
    //        {
    //            return;
    //        }

    //        if (_listener != null)
    //        {
    //            _listener.StopListening();
    //        }

    //        _socketClient.Close();
    //        _listener = null;
    //        _socketClient = null;
    //    }

    //    private void OnServerConnection()
    //    {
    //        if (Connected != null)
    //        {
    //            Connected.Invoke(_socketClient);
    //        }
    //    }

    //    private void OnMessageRecvied(Socket socket, byte[] buffer)
    //    {
    //        if (MessageRecived != null)
    //        {
    //            MessageRecived.Invoke(socket, buffer);
    //        }
    //    }

    //    private void OnServerConnectionDropped(Socket socket)
    //    {
    //        Close();
    //        RaiseServerDisconnected(socket);
    //    }

    //    private void RaiseServerDisconnected(Socket socket)
    //    {
    //        if (Disconncted != null)
    //        {
    //            Disconncted.Invoke(socket);
    //        }
    //    }
    //}
}