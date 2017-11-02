using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace Tcp_Protocol
{
    public class AbstractClient
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        private TcpClient DeviceClient;

        /// <summary>
        /// TCP客户端线程
        /// </summary>
        private Thread ClientRcvData;

        /// <summary>
        /// 接收缓存
        /// </summary>
        private byte[] rcvBuff = new byte[10 * 1024 * 1024];

        /// <summary>
        /// 设备接收数据异常委托
        /// </summary>
        /// <param name="ex"></param>
        public delegate void dltDeviceAccpetExCallBack(Exception ex);

        /// <summary>
        /// 数据接收委托
        /// </summary>
        /// <param name="len"></param>
        /// <param name="buffer"></param>
        public delegate void dltDeviceAccpetDataCallBack(int len, byte[] buffer);
        public dltDeviceAccpetDataCallBack dltDataCallBack;
        public dltDeviceAccpetExCallBack dltExCallBack;

        /// <summary>
        /// 服务器IP地址
        /// </summary>
        private IPAddress ConnectIP;

        /// <summary>
        /// 服务器端口号
        /// </summary>
        private int ConnectPort;

        /// <summary>
        /// 数据接收长度
        /// </summary>
        int isReadDataLen = 0;

        /// <summary>
        /// 初始化端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void InitSocket(IPAddress ip, int port)
        {
            ConnectIP = ip;
            ConnectPort = port;
        }

        /// <summary>
        /// TCP客户端线程
        /// </summary>
        private void RcvData()
        {
            while (true)
            {
                if (!isClientOnLine(DeviceClient))
                {
                    DeviceClient.Close();
                    DeviceClient = new TcpClient();
                    DeviceClient.ReceiveBufferSize = 10 * 1024 * 1024;
                    DeviceClient.Connect(ConnectIP, ConnectPort);
                }
                else
                {
                    if (DeviceClient.Available > 0)
                    {
                        try
                        {
                            isReadDataLen = DeviceClient.Client.Receive(rcvBuff, SocketFlags.None);
                            dltDataCallBack(isReadDataLen, rcvBuff);
                        }
                        catch (Exception ex)
                        {
                            dltExCallBack(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断客户端是否在线
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool isClientOnLine(TcpClient c)
        {
            return !((c.Client.Poll(1000, SelectMode.SelectRead) && (c.Client.Available == 0)) || !c.Client.Connected);
        }

        /// <summary>
        /// 开启客户端套接字
        /// </summary>
        public void StartSocket()
        {
            try
            {
                DeviceClient = new TcpClient();
                ClientRcvData = new Thread(RcvData);
                ClientRcvData.IsBackground = true;
                ClientRcvData.Start();
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 关闭客户端套接字
        /// </summary>
        public void StopSocket()
        {
            try
            {
                if (ClientRcvData.IsAlive)
                {
                    ClientRcvData.Abort();
                    DeviceClient.Close();
                }
                ClientRcvData = null;
                DeviceClient = null;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="str"></param>
        public void SendData(string str)
        {
            if (DeviceClient.Connected)
            {
                DeviceClient.Client.Send(System.Text.Encoding.Default.GetBytes(str));
            }
        }
    }
}
