using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Supervisor
{
    class TCPLineClient
    {
                private class ReceiveAlreadyInProgressException : Exception
        {
        }

        public enum StateType { NOT_CONNECTED, CONNECTING, CONNECTED };
        public string host;
        public string name;

        private Socket socket;
        private IPEndPoint remoteEP;

        private bool socketConnected = false;
        private bool receiveInProgress = false;
        private bool newData = false;

        private StateType state = StateType.NOT_CONNECTED;

        private Mutex mutex = new Mutex();

        private byte[] recvBuffer = new byte[256];
        private StringBuilder inData = new StringBuilder();
        private System.Collections.Generic.Queue<String> recvQueue = new System.Collections.Generic.Queue<String>();
        private System.Collections.Generic.Queue<String> sendQueue = new System.Collections.Generic.Queue<String>();

        ~TCPLineClient()
        {
            Close();
        }

        public StateType State()
        {
            return state;
        }

        // Returns true upon success (connect in progress) or false if the connection attempt cannot be initiated (invalid hostname?)
        public bool Init(string host, string name, int port)
        {
            this.host = host;
            this.name = name;
            // Connect to a remote device.
            try
            {
                Close();

                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.Resolve(host);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                socket.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), this);

                state = StateType.CONNECTING;

                return true;
            }
            catch (Exception e)
            {
                state = StateType.NOT_CONNECTED;

                return false;
            }
        }

        public void Update()
        {
            mutex.WaitOne();
            if (state == StateType.NOT_CONNECTED)
            {
                socketConnected = false;

                // Create a TCP/IP socket.
                socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                socket.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), this);

                state = StateType.CONNECTING;
            }
            else if (state == StateType.CONNECTING)
            {
                if (socketConnected)
                {
                    ConnectActions();
                    state = StateType.CONNECTED;
                }
            }
            else if (state == StateType.CONNECTED)
            {
                if (!receiveInProgress)
                {
                    if (newData)
                    {
                        int breakIndex;

                        while( (breakIndex = inData.ToString().IndexOf("\n")) != -1)
                        {
                            string message = inData.ToString().Substring(0, breakIndex);
                            inData.Remove(0, breakIndex + 1);

                            recvQueue.Enqueue(message);
                        }

                        newData = false;
                    }

                    Receive(socket);
                }

                while (sendQueue.Count > 0)
                {
                    Send(socket, sendQueue.Dequeue());
                }
            }
            mutex.ReleaseMutex();
        }

        // 
        public bool HasData()
        {
            if (recvQueue.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddToSendQueue(string message)
        {
            mutex.WaitOne();
            if (message[message.Length - 1] != '\n')
            {
                message = message + '\n';
            }
            sendQueue.Enqueue(message);
            mutex.ReleaseMutex();
        }

        public string Get()
        {
            mutex.WaitOne();
            if (recvQueue.Count > 0)
            {
                string message = recvQueue.Dequeue();
                mutex.ReleaseMutex();
                return message;
            }
            else
            {
                mutex.ReleaseMutex();
                return null;
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            TCPLineClient client = (TCPLineClient) ar.AsyncState;

            try
            {
                // Retrieve the socket from the state object.
                Socket socket = client.socket;

                // Complete the connection.
                socket.EndConnect(ar);

                // Signal that the connection has been made.
                client.socketConnected = true;
            }
            catch (Exception e)
            {
                client.Close();
            }
        }

        private void ConnectActions() {
	        // nothing here yet
        }

        private bool Send(Socket client, String data)
        {
            try
            {
                // Convert the string data to byte data using ASCII encoding.
                byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.
                client.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), this);

                return true;
            }
            catch (Exception e)
            {
                Close();
                return false;
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            TCPLineClient client = (TCPLineClient)ar.AsyncState;

            try
            {
                // Retrieve the socket from the state object.
                Socket socket = client.socket;

                // Complete sending the data to the remote device.
                int bytesSent = socket.EndSend(ar);
            }
            catch (Exception e)
            {
                client.Close();
            }
        }

        private void Receive(Socket client)
        {
            if (receiveInProgress)
            {
                throw new ReceiveAlreadyInProgressException();
            }
            else
            {
                try
                {
                    receiveInProgress = true;

                    // Begin receiving the data from the remote device.
                    client.BeginReceive(recvBuffer, 0, recvBuffer.Length, 0,
                        new AsyncCallback(ReceiveCallback), this);
                }
                catch (Exception e)
                {
                    Close();
                }
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            TCPLineClient client = (TCPLineClient)ar.AsyncState;

            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                Socket socket = client.socket;

                // Read data from the remote device.
                int bytesRead = socket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    client.inData.Append(Encoding.ASCII.GetString(client.recvBuffer, 0, bytesRead));

                    // Signal that all bytes have been received.
                    client.receiveInProgress = false;
                    client.newData = true;
                }
                else
                {
                    client.Close();
                }
            }
            catch (Exception e)
            {
                client.Close();
            }
        }

        public void Close()
        {
            state = StateType.NOT_CONNECTED;
            socketConnected = false;
            receiveInProgress = false;

            if (socket != null)
            {
                try
                {
                    // Release the socket.
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
