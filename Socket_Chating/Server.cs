using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Socket_Chating
{
    class Server
    {
        class AsyncObject{
            public byte[] Buffer;
            public Socket WorkingSocket;
            public AsyncObject(int size){
                Buffer = new byte[size];
            }
        }
        
        private static Socket _serverSocket = null; // server에서 쓸 소켓이다.
        private static AsyncCallback _acceptHandle = new AsyncCallback(HandleClientConnectionRequest);
        private static AsyncCallback _receiveHandler = new AsyncCallback(HandleDataReceive);
        private static AsyncCallback _sendHandler = new AsyncCallback(HandleDataSend);

        private const int Port = 6974;
        private const int Backlog = 4;

        public void StartServer()
        {
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            _serverSocket.Listen(Backlog);
            _serverSocket.BeginAccept(_acceptHandle, null);
        }

        public IAsyncResult TryAccept()
        {
            return _serverSocket.BeginAccept(_acceptHandle, null);
        }

        public void SendMessage(String msg)
        {
            AsyncObject ao = new AsyncObject(1);
            ao.Buffer = Encoding.Unicode.GetBytes(msg);
            _serverSocket.BeginSend(
                ao.Buffer, 
                0, 
                ao.Buffer.Length, 
                SocketFlags.None, 
                _sendHandler, ao);
        }

        private static void HandleClientConnectionRequest(IAsyncResult ar)
        {
            Socket clientSocket = _serverSocket.EndAccept(ar);
            AsyncObject asyncObject = new AsyncObject(2048);
            asyncObject.WorkingSocket = clientSocket;

            clientSocket.BeginReceive(
                asyncObject.Buffer, 
                0, 
                asyncObject.Buffer.Length, 
                SocketFlags.None,
                _receiveHandler, 
                asyncObject);
        }

        private static void HandleDataReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;

            int receiveBytes = ao.WorkingSocket.EndReceive(ar);
            
            if (receiveBytes > 0)
            {
                Console.WriteLine("Received MSG : {0}",Encoding.Unicode.GetString(ao.Buffer));
            }

            ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, _receiveHandler, ao);
        }

        private static void HandleDataSend(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;

            int sentBytes = ao.WorkingSocket.EndReceive(ar);

            if (sentBytes > 0)
            {
                Console.WriteLine("Sent MSG: {0}",Encoding.Unicode.GetString(ao.Buffer));
            }
        }
    }
}