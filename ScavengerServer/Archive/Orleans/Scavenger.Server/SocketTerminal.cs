using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server
{
    public sealed class ServerTerminal
    {
        Socket _socket;
        TcpListener _listener;
        TcpClient _client;

        private bool _isClosed;
        private bool _isListening;
        private Socket _workerSocket;

        public event Func<TcpClient, CancellationToken, Task> HandleClient;

        public async Task StartListen(IPEndPoint endPoint, CancellationToken cancellationToken)
        {
            //_socket = new Socket(AddressFamily.InterNetwork,
            //    SocketType.Stream, ProtocolType.Tcp);

            _listener = new TcpListener(endPoint);

            try
            {
                _listener.Start();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    HandleClientAsync(client, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
        {
            if (HandleClient != null)
                await HandleClient.Invoke(client, cancellationToken);
        }

        //private void OnClientConnection(IAsyncResult asyn)
        //{
        //    if (_isClosed)
        //    {
        //        return;
        //    }

        //    try
        //    {
        //        _workerSocket = _socket.EndAccept(asyn);

        //        RaiseClientConnected(_workerSocket);

        //        _listener = new SocketListener();
        //        _listener..MessageReceived += OnMessageRecived;
        //        _listener.Disconnected += OnClientDisconnection;

        //        _listener.StartReceiving(_workerSocket);
        //    }
        //    catch (ObjectDisposedException odex)
        //    {
        //        Debug.Fail(odex.ToString(),
        //            "OnClientConnection: Socket has been closed");
        //    }
        //    catch (Exception sex)
        //    {
        //        Debug.Fail(sex.ToString(),
        //            "OnClientConnection: Socket failed");
        //    }

        //}

        //private void OnClientDisconnection(Socket socket)
        //{
        //    RaiseClientDisconnected(socket);

        //    // Try to re-establish connection
        //    _socket.BeginAccept(new AsyncCallback(OnClientConnection), null);

        //}
        public void SendMessage(byte[] buffer)
        {
            if (_workerSocket == null)
            {
                return;
            }
            _workerSocket.Send(buffer);
        }
        
        internal void StopListen()
        {
            _listener.Stop();
        }
    }
}
