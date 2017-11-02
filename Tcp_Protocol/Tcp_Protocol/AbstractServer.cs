using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace Tcp_Protocol
{
    public class AbstractServer
    {
        private TcpListener listener;
        private TcpClient devcieClient;
        private int port;
        private Thread listenerThread;
        private int isReadDataLen = 0;
        private byte[] RcvBuff = new byte[10 * 1024 * 1024];
        public delegate void dltRcvClientData(int len, byte[] buffer);
        public dltRcvClientData dltClientData;

        public AbstractServer(int port)
        {
            this.port = port;
            listenerThread = new Thread(ListenerClinet);
            listenerThread.IsBackground = true;
            devcieClient = new TcpClient();
        }
        public void ListenerClinet()
        {
            while (true)
            {
                try
                {
                    if (!isClientInOnline(devcieClient))
                    {
                        devcieClient = listener.AcceptTcpClient();
                        devcieClient.Client.ReceiveBufferSize = 10 * 1024 * 1024;
                        devcieClient.Client.SendTimeout = 1000;
                        continue;
                    }
                    if (devcieClient != null && devcieClient.Client.Available > 0)
                    {
                        isReadDataLen = devcieClient.Client.Receive(RcvBuff, SocketFlags.None);
                        dltClientData(isReadDataLen, RcvBuff);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        public bool isClientInOnline(TcpClient client)
        {
            return !((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
        }

        public void SendData(string str)
        {
            if (devcieClient.Client.Connected)
            {
                devcieClient.Client.Send(System.Text.Encoding.Default.GetBytes(str));
            }
        }

        public void StartSocket()
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                listenerThread.Start();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
