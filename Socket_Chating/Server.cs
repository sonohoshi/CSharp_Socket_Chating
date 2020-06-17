using System;
using System.Net;
using System.Net.Sockets;

namespace Socket_Chating
{
    class Server
    {
        private Socket serverSocket = null; // server에서 쓸 소켓이다.
        private AsyncCallback acceptHandle = new AsyncCallback(HandleClientConnectionRequest);

        private const int port = 6974;
        private const int backlog = 4;

        public void StartServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            
            serverSocket.Listen(backlog);

            serverSocket.BeginAccept(acceptHandle, null);
        }

        private void HandleClientConnectionRequest(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
        }
    }
}