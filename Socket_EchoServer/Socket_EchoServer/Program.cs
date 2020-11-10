using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        
        private const int port = 6974;

        public static void StartListening()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndpoint = new IPEndPoint(ipAddress, port);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndpoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();
                    
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallBack), listener);

                    allDone.WaitOne();
                }
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            Console.WriteLine("\nPress ENTER to continue...");  
            Console.Read();
        }
        
        public static void AcceptCallBack(IAsyncResult asyncResult)
        {
            allDone.Set();

            Socket listener = (Socket) asyncResult.AsyncState;
            Socket handler = listener.EndAccept(asyncResult);
            
            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, SocketFlags.None,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult asyncResult)
        {
            String content = String.Empty;

            StateObject state = (StateObject) asyncResult.AsyncState;
            Socket handler = state.WorkSocket;

            int bytesRead = handler.EndReceive(asyncResult);

            if (bytesRead > 0)
            {
                state.SBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                content = state.SBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    Send(handler, content);
                }
            }
            else
            {
                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback),
                    state);
            }
        }
        
        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);  
  
            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,  
                new AsyncCallback(SendCallback), handler);  
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket) ar.AsyncState;  
  
                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);  
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);  
  
                handler.Shutdown(SocketShutdown.Both);  
                handler.Close();  
  
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());  
            }  
        }

        
        public static void Main(string[] args)
        {
            StartListening();
        }
    }
}