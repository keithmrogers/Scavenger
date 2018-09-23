using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.IO;

namespace Scavenger.Server
{
    public class SocketListener
    {
        private const int BufferSize = 1024;
        AsyncCallback _workerCallBack;
        Socket _workSocket;

        public event Action<Socket, byte[]> MessageReceived;
        public event Action<Socket> Disconnected;

        public void StartReceiving(Socket socket)
        {
            _workSocket = socket;
            WaitForData(socket);
        }

        private void WaitForData(System.Net.Sockets.Socket socket)
        {
            try
            {
                if (_workerCallBack == null)
                {
                    _workerCallBack = new AsyncCallback(OnDataReceived);
                }

                var stateObject = new StateObject(BufferSize);
                stateObject.workSocket = socket;

                socket.BeginReceive(
                    stateObject.buffer,
                    0,
                    stateObject.buffer.Length,
                    SocketFlags.None,
                    _workerCallBack,
                    stateObject);
            }
            catch (SocketException sex)
            {
                //Debug.Fail(sex.ToString(), "WaitForData: Socket failed");
            }

        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            var state = (StateObject)asyn.AsyncState;
            var socket = state.workSocket;

            if (!socket.Connected)
            {
                return;
            }

            try
            {
                int iRx;
                try
                {
                    iRx = socket.EndReceive(asyn);
                }
                catch (SocketException)
                {
                    //Debug.Write("Apperently client has been closed and connot answer.");

                    OnConnectionDropped(socket);
                    return;
                }

                if (iRx == 0)
                {
                    //Debug.Write("Apperently client socket has been closed.");

                    OnConnectionDropped(socket);
                    return;
                }

                RaiseMessageRecived(state.buffer);

                WaitForData(_workSocket);
            }
            catch (Exception ex)
            {
                //Debug.Fail(ex.ToString(), "OnClientConnection: Socket failed");
            }
        }

        public void StopListening()
        {
            if (_workSocket != null)
            {
                _workSocket.Close();
                _workSocket = null;
            }
        }

        private void RaiseMessageRecived(byte[] buffer)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(_workSocket, buffer);
            }
        }

        private void OnDisconnection(Socket socket)
        {
            if (Disconnected != null)
            {
                Disconnected.Invoke(socket);
            }
        }

        private void OnConnectionDropped(Socket socket)
        {
            _workSocket = null;
            OnDisconnection(socket);
        }

        private class StateObject
        {
            public StateObject(int buffersize)
            {
                buffer = new byte[buffersize];
            }
            // Client  socket.
            public Socket workSocket;
            // Receive buffer.
            public byte[] buffer;
            // Received data string.
            public byte[] data;
            //public StringBuilder sb = new StringBuilder();
        }
    }
}