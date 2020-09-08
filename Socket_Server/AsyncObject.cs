using System.Net.Sockets;

namespace Socket_Server
{
    public class AsyncObject
    {
        public byte[] Buffer;
        public Socket WorkingSocket;
        public AsyncObject(int size){
            Buffer = new byte[size];
        }
    }
}