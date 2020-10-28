using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Socket_EchoServer
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder SBuilder = new StringBuilder();
        public Socket WorkSocket = null;
    }
    
    internal class Program
    {
        private const int port = 6974; 
        
        public static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, port);
            
        }
    }
}