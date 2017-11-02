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
        /// <summary>
        /// TCP监听
        /// </summary>
        private TcpListener listener;
        
        /// <summary>
        /// 监听客户端
        /// </summary>
        private TcpClient DevcieClient;

        /// <summary>
        /// 端口号
        /// </summary>
        private int port;

        /// <summary>
        /// 监听线程
        /// </summary>
        private Thread listenerThread;

        /// <summary>
        /// 接收数据长度
        /// </summary>
        private int isReadDataLen = 0;

        /// <summary>
        /// 数据缓存
        /// </summary>
        private byte[] RcvBuff = new byte[10 * 1024 * 1024];

        public delegate void dltRcvClientData(int len, byte[] buffer);
        public dltRcvClientData dltClientData;

        public AbstractServer(int port)
        {
            this.port = port;
            listenerThread = new Thread(ListenerClinet);
            listenerThread.IsBackground = true;
            DevcieClient = new TcpClient();
        }
        public void ListenerClinet()
        {
            while (true)
            {
                try
                {
                    if (!isClientInOnline(DevcieClient))
                    {
                        DevcieClient = listener.AcceptTcpClient();
                        DevcieClient.Client.ReceiveBufferSize = 10 * 1024 * 1024;
                        DevcieClient.Client.SendTimeout = 1000;
                        continue;
                    }
                    if (DevcieClient != null && DevcieClient.Client.Available > 0)
                    {
                        isReadDataLen = DevcieClient.Client.Receive(RcvBuff, SocketFlags.None);
                        dltClientData(isReadDataLen, RcvBuff);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// 判断客户端是否在线
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool isClientInOnline(TcpClient client)
        {
            return !((client.Client.Poll(1000, SelectMode.SelectRead) && (client.Client.Available == 0)) || !client.Client.Connected);
        }

        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="str"></param>
        public void SendData(string str)
        {
            if (DevcieClient.Client.Connected)
            {
                DevcieClient.Client.Send(System.Text.Encoding.Default.GetBytes(str));
            }
        }

        /// <summary>
        /// 开启服务器套接字
        /// </summary>
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
