using System.Net.Sockets;
using System.Text;

namespace Socket_Client
{
    public class StateObject
    {
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder sBuilder = new StringBuilder();
        public Socket workSocket = null;
    }
    internal class Program
    {
        public static void Main(string[] args)
        {
            
        }
    }
}