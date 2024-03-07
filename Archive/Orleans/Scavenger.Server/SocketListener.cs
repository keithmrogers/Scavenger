using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server
{
    public class SocketListener
    {
        private const int BufferLength = 1000;
        AsyncCallback pfnWorkerCallBack;
        Socket m_socWorker;

        public event Action<Socket, byte[]> MessageReceived;
        public event Action<Socket> Disconnected;

        public void StartReceiving(Socket socket)
        {
            m_socWorker = socket;
            WaitForData(socket);
        }

        private void WaitForData(System.Net.Sockets.Socket soc)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }

                var stateObject = new StateObject(BufferLength);
                stateObject.workSocket = soc;

                soc.BeginReceive(
                    stateObject.buffer,
                    0,
                    stateObject.buffer.Length,
                    SocketFlags.None,
                    pfnWorkerCallBack,
                    stateObject);
            }
            catch (SocketException sex)
            {
                Debug.Fail(sex.ToString(), "WaitForData: Socket failed");
            }

        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            var stateObject = (StateObject)asyn.AsyncState;
            Socket socket = stateObject.workSocket;

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
                    Debug.Write("Apperently client has been closed and connot answer.");

                    OnConnectionDroped(socket);
                    return;
                }

                if (iRx == 0)
                {
                    Debug.Write("Apperently client socket has been closed.");

                    OnConnectionDroped(socket);
                    return;
                }

                RaiseMessageRecived(socket, stateObject.buffer);

                WaitForData(m_socWorker);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString(), "OnClientConnection: Socket failed");
            }
        }

        public void StopListening()
        {
            if (m_socWorker != null)
            {
                m_socWorker.Close();
                m_socWorker = null;
            }
        }

        private void RaiseMessageRecived(Socket socket, byte[] buffer)
        {
            MessageReceived?.Invoke(socket, buffer);
        }

        private void OnDisconnection(Socket socket)
        {
            Disconnected?.Invoke(socket);
        }

        private void OnConnectionDroped(Socket socket)
        {
            m_socWorker = null;
            OnDisconnection(socket);
        }
    }
}
