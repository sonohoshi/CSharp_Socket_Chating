using System;
using System.Net;
using System.Net.Sockets;

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
            
        }
    }
}