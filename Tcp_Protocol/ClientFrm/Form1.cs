using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tcp_Protocol;
using System.Net;

namespace ClientFrm
{
    public partial class Form1 : Form
    {
        AbstractClient Client;
        AbstractServer Server;
        public Form1()
        {
            InitializeComponent();
            Client = new AbstractClient();
            Client.InitSocket(IPAddress.Parse("127.0.0.1"), 5000);
            Client.dltDataCallBack += new AbstractClient.dltDeviceAccpetDataCallBack(RcvDataCallBack);

            Server = new AbstractServer(5000);
            Server.dltClientData += new AbstractServer.dltRcvClientData(RcvDataServerCallBack);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.StartSocket();
        }
        void RcvDataCallBack(int len, byte[] recbuff)
        {
            if (len != 0)
            {
                string str = System.Text.Encoding.Default.GetString(recbuff, 0, len);
                this.Invoke(new Action(() => { label1.Text = str; }));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Client.StopSocket();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Client.SendData(this.textBox1.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Server.StartSocket();
        }
        void RcvDataServerCallBack(int len, byte[] buffer)
        {
            if (len != 0)
            {
                string str = System.Text.Encoding.Default.GetString(buffer, 0, len);
                this.Invoke(new Action(() => { label2.Text = str; }));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Server.SendData(this.textBox2.Text);
        }
    }
}
