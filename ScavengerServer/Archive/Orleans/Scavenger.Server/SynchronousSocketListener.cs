using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Scavenger.Server
{
    public class StateObject
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

    public class SynchronousSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public event LogMessageDelegate OnLogMessage;
        public delegate void LogMessageDelegate(object sender, LogMessageEventArgs e);
        
        public event ReceivedMessageDelegate OnReceivedMessage;
        public delegate void ReceivedMessageDelegate(object sender, ReceivedMessageEventArgs e);

        public void StartListening(IPEndPoint endPoint)
        {
            // Data buffer for incoming data
            byte[] bytes = new Byte[1024];

            //Create a TCP/IP socket.
            var listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(endPoint);
                listener.Listen(100);

                LogMessage(string.Format("Listening on {0}", endPoint.ToString()));

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                LogMessage(string.Format("Error: {0}", e.ToString()));
            }
        }

        private void LogMessage(string message)
        {
            OnLogMessage?.Invoke(this, new LogMessageEventArgs() { Message = message });
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject(1024);
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, state.buffer.Length, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);
            int newDataPoint = 0;

            if (state.data == null)
            {
                state.data = new byte[bytesRead];
            }
            else if(bytesRead != 0)
            {
                //this can be more efficient probably
                newDataPoint = state.data.Length;
                byte[] tempStorage = new byte[state.data.Length];
                state.data.CopyTo(tempStorage, 0);

                state.data = new byte[state.data.Length + bytesRead];
                tempStorage.CopyTo(state.data, 0);
            }
            
            if (bytesRead > 0)
            {
                if (state.data.Length > 1024)
                {
                    state.buffer.CopyTo(state.data, newDataPoint);
                }
                else
                {
                    //first transfer
                    state.data = state.buffer;
                }
                
                handler.BeginReceive(state.buffer, 0, 1024, 0,
                    new AsyncCallback(ReadCallback), state);
            }
            else
            {
                OnReceivedMessage?.Invoke(this, new ReceivedMessageEventArgs() { ConnectedSocket = state.workSocket, Data = state.data });
            }
        }

        private void AcceptCallback(object sender, SocketAsyncEventArgs e)
        {
            LogMessage("Message Received");
            Socket listenSocket = (Socket)sender;
            do
            {
                try
                {
                    Socket newSocket = e.AcceptSocket;
                    Debug.Assert(newSocket != null);

                    byte[] bytes = new Byte[1024];

                    int bytesRec = newSocket.Receive(bytes);
                    
                    OnReceivedMessage?.Invoke(sender, new ReceivedMessageEventArgs() { Data = bytes, ConnectedSocket = newSocket });
                    
                    //var messageWrapper = new MessageWrapper() { MessageType = MessageType.InfoOnlyMessage };
                    //var messageData = new InfoOnlyMessage() { Message = "Received!" };

                    //messageWrapper.Data = messageData;

                    //newSocket.Send(messageWrapper.ObjectToByteArray());
                    //newSocket.Disconnect(false);
                    //newSocket.Close();
                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message);
                }
                finally
                {
                    //to enable reuse
                    e.AcceptSocket = null; 
                }
            } while (!listenSocket.AcceptAsync(e));
        }
    }

    public class LogMessageEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ReceivedMessageEventArgs : EventArgs
    {
        public Socket ConnectedSocket { get; set; }
        public byte[] Data { get; set; }
    }
}
