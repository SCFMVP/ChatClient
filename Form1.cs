using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//添加新的名称空间引用
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//禁用此异常
        }

        #region 变量
        //客户机与服务器之间的连接状态
        public bool bConnected = false;
        //侦听线程
        public Thread tAcceptMsg = null;
        //用于Socket通信的IP地址和端口
        public IPEndPoint IPP = null;
        //Socket通信
        public Socket socket = null;
        //网络访问的基础数据流
        public NetworkStream nStream = null;
        //创建读取器
        public TextReader tReader = null;
        //创建编写器
        public TextWriter wReader = null;

        char[] tmp_v_rec_buff = new char[4096];  //接收socket缓存
        char[] p_jpeg_buff = new char[1024 * 1024];
        int jpeg_pos = 0;      //bi
        #endregion

        //显示信息
        public void AcceptMessage()
        {
            string sTemp; //临时存储读取的字符串
            int i;
            while (bConnected)
            {
                try
                {
                    //连续从当前流中读取字符串直至结束
                    i=tReader.ReadBlock(tmp_v_rec_buff,0,4096);
                    if (i != 0)
                    {
                        //richTextBox2_KeyPress()和AcceptMessage()
                        //都将向richTextBox1写字符，可能访问有冲突，
                        //所以，需要多线程互斥
                        lock (this)
                        {
                            richTextBox1.Text = "服务器：" + tmp_v_rec_buff + "\n" + richTextBox1.Text;
                            draw_jpeg(tmp_v_rec_buff, i);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("无法与服务器通信。");
                }
            }
            //禁止当前Socket上的发送与接收
            socket.Shutdown(SocketShutdown.Both);
            //关闭Socket，并释放所有关联的资源
            socket.Close();
        }
        //把socket接收的数据提取jpg头尾标志(ffd8...ffd9)
        void draw_jpeg(char[] data, int len)
        {
            int i;
            for (i = 0; i < len; i++)
            {
                //jpg头
                if (data[i] == 0xff && data[i + 1] == 0xd8)
                {
                    jpeg_pos = 0;
                }
                p_jpeg_buff[jpeg_pos++] = data[i];
                if (jpeg_pos >= 1024 * 1024)
                {
                    jpeg_pos = 0;
                    break;
                }
                //jpg尾
                if (jpeg_pos >= 2 &&
                    p_jpeg_buff[jpeg_pos - 2] == 0xff &&
                    p_jpeg_buff[jpeg_pos - 1] == 0xd9 &&
                    p_jpeg_buff[0] == 0xff &&
                    p_jpeg_buff[1] == 0xd8)
                {
                    pictureBox1.Image=ByteArray2Image(HexString2Bytes(p_jpeg_buff));                    
                }

            }
        }

        //创建与服务器的连接，侦听并显示聊天信息
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPP);
                if (socket.Connected)
                {
                    nStream = new NetworkStream(socket);
                    tReader = new StreamReader(nStream);
                    wReader = new StreamWriter(nStream);
                    tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
                    tAcceptMsg.Start();
                    bConnected = true;
                    button1.Enabled = false;
                    MessageBox.Show("与服务器成功建立连接，可以通信。");
                }
            }
            catch
            {
                MessageBox.Show("无法与服务器通信。");
            }
        }

        //发送信息
        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)//按下的是回车键
            {
                if (bConnected)
                {
                    try
                    {
                        //richTextBox2_KeyPress()和AcceptMessage()
                        //都将向richTextBox1写字符，可能访问有冲突，
                        //所以，需要多线程互斥
                        lock (this)
                        {
                            richTextBox1.Text = "客户机：" + richTextBox2.Text + richTextBox1.Text;
                            //客户机聊天信息写入网络流，以便服务器接收
                            wReader.WriteLine(richTextBox2.Text);
                            //清理当前缓冲区数据，使所有缓冲数据写入基础设备
                            wReader.Flush();
                            //发送成功后，清空输入框并聚集之
                            richTextBox2.Text = "";
                            richTextBox2.Focus();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("与服务器连接断开。");
                    }
                }
                else
                {
                    MessageBox.Show("未与服务器建立连接，不能通信。");
                }
            }
        }
        #region[hex2pic]
        /// <summary>
        /// hex字符串to字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(String hexString)
        {
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException("参数长度不正确");
            }

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }
        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="Bytes">字节数组</param>
        /// <returns>图片</returns>
        private Image ByteArray2Image(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                Image outputImg = Image.FromStream(ms);
                return outputImg;
            }
        }
        #endregion

        //关闭窗体时断开socket连接，并终止线程（否则，VS调试程序将仍处于运行状态）
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.Close();
                tAcceptMsg.Abort();
            }
            catch
            { }
        }
    }
}
