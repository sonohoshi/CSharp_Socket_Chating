using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Socket_Chating
{
    public class Client
    {
        private string hostAd = "";
        private int port = 6974;
        private Socket _client = null; // 클라이언트에서 쓸 소켓이다.
        
        private AsyncCallback receiveHandler = new AsyncCallback(HandleDataReceive);
        private AsyncCallback sendHandler = new AsyncCallback(HandleDataSend);

        public void ConnectToServer(String host, UInt16 port)
        {
            _client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bool isConnected = false;
            try
            {
                _client.Connect(host, port);
                isConnected = true;
            }
            catch
            {
                isConnected = false;
            }

            if (isConnected)
            {
                AsyncObject ao = new AsyncObject(5000);
                ao.WorkingSocket = _client;

                _client.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, receiveHandler, ao);
            }
            else
            {
                throw new InvalidOperationException("Can't connect to server");
            }
        }

        public void SendMessage(String sendMsg)
        {
            AsyncObject ao = new AsyncObject(1);

            ao.Buffer = Encoding.UTF8.GetBytes(sendMsg);
            ao.WorkingSocket = _client;

            try
            {
                _client.BeginSend(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, sendHandler, ao);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Sending\nMSG : {0}", e.Message);
            }
        }

        private static void HandleDataReceive(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;
            Int32 receivedBytes = ao.WorkingSocket.EndReceive(ar);

            if (receivedBytes > 0)
            {
                Byte[] messageBytes = new Byte[receivedBytes];
                Array.Copy(ao.Buffer,messageBytes,receivedBytes);
                Console.WriteLine("Received MSG : {0}",Encoding.UTF8.GetString(messageBytes));
            }
        }

        private static void HandleDataSend(IAsyncResult ar)
        {
            AsyncObject ao = (AsyncObject) ar.AsyncState;
            Int32 sendBytes;

            try
            {
                sendBytes = ao.WorkingSocket.EndReceive(ar);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in Sending : {0}",e.Message);
                return;
            }

            if (sendBytes > 0)
            {
                Byte[] messageBytes = new byte[sendBytes];
                Array.Copy(ao.Buffer, messageBytes, sendBytes);
                
                Console.WriteLine("Sent MSG : {0}", Encoding.Unicode.GetString(messageBytes));
            }
        }
    }
}