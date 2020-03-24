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
//todo:服务端停止,我这里才能收到消息
namespace ChatClient
{
    public partial class Form1 : Form
    {

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


        char[] p_jpeg_buff = new char[1024 * 1024];//接收socket缓存
        string imgHexStr = null;
        int jpeg_pos = 0;
        string receData = "";
        string finalData = "";
        #endregion

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//禁用此异常    
            //直接测试显示ok    
            //pictureBox1.Image = ByteArray2Image(HexString2Bytes(JpegData.pic));
            textBox1.Text = JpegData.SERVER_ADDRESS;
            textBox2.Text = JpegData.SEVER_PORT;

        }

        //显示信息
        public void AcceptMessage()
        {
           // string sTemp; //临时存储读取的字符串
            while (bConnected)
            {
                try
                {
                    //定义1M缓冲区
                    byte[] arrRecMsg = new byte[1024 * 1024];
                    //将客户端套接字接收到的数据存入内存缓冲区, 并获取其长度
                    int length = socket.Receive(arrRecMsg);
                    
                    string sTemp = Encoding.UTF8.GetString(arrRecMsg);
                    //连续从当前流中读取字符串直至结束
                    //sTemp =tReader.ReadLine();
                    //sTemp=StringToHexArray(sTemp);    //转换成16进制str
                    if (sTemp.Length != 0)
                    {
                        lock (this)
                        {
                            richTextBox1.Text = "服务器：" + sTemp + "\n" + richTextBox1.Text;
                            //draw_jpeg(sTemp, sTemp.Length);
                            Console.WriteLine("sTempLenth: "+sTemp.Length);
                        }
                    }
                    if (receData != null && receData.Contains("FF D8") && receData.Contains("FE D9"))
                    {
                        //完整数据包
                        draw_jpeg(receData, receData.Length);
                        //finalData = receData;
                        receData = "";      //清除接收
                        receData += sTemp;
                    }
                    else
                    {
                        //不完整数据包
                        receData += sTemp;
                        //Log.d(TAG, "receData_temp:" + sTemp);

                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine("无法与服务器通信: "+e.ToString());
                    //MessageBox.Show("无法与服务器通信。");
                }
            }
            //禁止当前Socket上的发送与接收
            socket.Shutdown(SocketShutdown.Both);
            //关闭Socket，并释放所有关联的资源
            socket.Close();
        }
        //把socket接收的数据提取jpg头尾标志(ffd8...ffd9)
        void draw_jpeg(String data, int len)
        {
            int i;
            Console.WriteLine("data[1]:"+data[1]);
            for (i = 0; i < len; i++)
            {
                //在数据包中检测到jpg头
                if (data[i] == 'F' && data[i + 1] == 'F' && data[i + 2] == 'D' && data[i + 3] == '8')
                {
                    jpeg_pos = 0;  //jpeg_buffer数组标号
                }
                //循环写入新的数组(分离出来独立的jpeg数据)
                //p_jpeg_buff[jpeg_pos++] = data[i];
                imgHexStr += data[i];
                jpeg_pos++;
                if (jpeg_pos >= 1024 * 1024)
                {
                    jpeg_pos = 0;
                    break;
                }
                //在数据包中检测到jpg尾
                //if (jpeg_pos >= 4 &&
                //    p_jpeg_buff[jpeg_pos - 3] == 'F' && p_jpeg_buff[jpeg_pos - 4] == 'F' &&
                //    p_jpeg_buff[jpeg_pos - 1] == '9' && p_jpeg_buff[jpeg_pos - 2] == 'D' &&
                //    p_jpeg_buff[0] == 'F' && p_jpeg_buff[1] == 'F' && p_jpeg_buff[2] == 'D' && p_jpeg_buff[3] == '8')
                if (jpeg_pos >= 4 &&
                    imgHexStr[jpeg_pos - 3] == 'F' && imgHexStr[jpeg_pos - 4] == 'F' &&
                    imgHexStr[jpeg_pos - 1] == '9' && imgHexStr[jpeg_pos - 2] == 'D' &&
                    imgHexStr[0] == 'F' && imgHexStr[1] == 'F' && imgHexStr[2] == 'D' && imgHexStr[3] == '8')
                {
                    //pictureBox1.Image=ByteArray2Image(Char2Byte(p_jpeg_buff));    
                    pictureBox1.Image = ByteArray2Image(HexString2Bytes(imgHexStr));
                }

            }
        }
        #region[hex2pic]
        /// <summary>
        /// Char2Byte
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static byte[] Char2Byte(char[] c)
        {
            int i = 0;
            byte[] b = new byte[c.Length * 2];
            for (i = 0; i < c.Length; i++)
            {
                b[2 * i] = (byte)((c[i] & 0xFF00) >> 8);
                b[2 * i + 1] = (byte)(c[i] & 0xFF);
            }
            return b;
        }
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

        /// <summary>
        /// str2hexStr
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StringToHexArray(string input)
        {
            string result = string.Empty;
            byte[] b = Encoding.Default.GetBytes(input);//按照指定编码将string编程字节数组
            for (int i = 0; i < b.Length; i++)
            {
                result += (b[i].ToString("X2") + ' ');
            }          
            return result;
        }
    #endregion

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
        

        //关闭窗体时断开socket连接，并终止线程（否则，VS调试程序将仍处于运行状态）
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.Close();
                tAcceptMsg.Abort();
                //todo:关闭线程
            }
            catch
            { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
