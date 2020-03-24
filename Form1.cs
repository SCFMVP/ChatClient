﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        private Socket MySocket = null;// Socket

        public const int TCPBufferSize = 1460;//缓存的最大数据个数
        public byte[] TCPBuffer = new byte[TCPBufferSize];//缓存数据的数组
        string receData = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "192.168.0.8";
            textBox2.Text = "8089";
            checkBox1.Checked = true;
        }


        /// <连接按钮点击事件>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "连接"){
                //IP地址 和 端口号输入不为空
                if (string.IsNullOrEmpty(textBox1.Text) == false && string.IsNullOrEmpty(textBox2.Text) == false){
                    try{
                        IPAddress ipAddress = IPAddress.Parse(textBox1.Text);//获取IP地址
                        int Port = Convert.ToInt32(textBox2.Text);          //获取端口号
                        MySocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
                        //使用 BeginConnect 异步连接
                        MySocket.BeginConnect(ipAddress, Port, new AsyncCallback(ConnectedCallback), MySocket);
                    }
                    catch (Exception){
                        MessageBox.Show("IP地址或端口号错误!", "提示");
                    }
                }
                else{
                    MessageBox.Show("IP地址或端口号错误!", "提示");
                }
            }
            else
            {
                try{

                    MySocket.BeginDisconnect(false,null,null);//断开连接
                    button1.Text = "连接";
                }
                catch (Exception){}
            }
        }

        /// <连接异步回调函数>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        void ConnectedCallback(IAsyncResult ar)
        {
            //无法再次连接
            Socket socket = (Socket)ar.AsyncState;//获取Socket
            try{
                socket.EndConnect(ar);
                //设置异步读取数据,接收的数据缓存到TCPBuffer,接收完成跳转ReadCallback函数
                socket.BeginReceive(TCPBuffer, 0, TCPBufferSize, 0,new AsyncCallback(ReadCallback), socket);
                Invoke((new Action(() =>
                {
                    textBox3.AppendText("成功连接服务器\n");//对话框追加显示数据
                    button1.Text = "断开";
                })));
            }
            catch (Exception e){
                Invoke((new Action(() =>
                {
                    textBox3.AppendText("连接失败:" + e.ToString());//对话框追加显示数据
                })));
            }
        }

        /// <接收到数据回调函数>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        void ReadCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;//获取链接的Socket
            int CanReadLen = socket.EndReceive(ar);//结束异步读取回调,获取读取的数据个数
            //读到数据
            if (CanReadLen > 0)
            {
                Invoke((new Action(() => //C# 3.0以后代替委托的新方法
                {
                    if (checkBox1.Checked)//16进制显示
                    {
                        //做数据处理
                        string sTemp = byteToHexStr(TCPBuffer, CanReadLen);   //获得hexStr
                        textBox3.AppendText(byteToHexStr(TCPBuffer, CanReadLen));//对话框追加显示数据
                        //含有一个完整包(包含空格)
                        if (receData != null && receData.Contains("FF D8") && receData.Contains("FF D9"))
                        {
                            //完整数据包
                            receData=GetFullImg(receData);
                            //draw_jpeg(receData, receData.Length);
                            pictureBox1.Image = ByteArray2Image(HexString2Bytes(receData));
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
                    else
                    {
                        textBox3.AppendText(Encoding.Default.GetString(TCPBuffer, 0, CanReadLen));//对话框追加显示数据
                    }
                })));
                //设置异步读取数据,接收的数据缓存到TCPBuffer,接收完成跳转ReadCallback函数
                if(button1.Text != "连接")
                    socket.BeginReceive(TCPBuffer,0, TCPBufferSize, 0, new AsyncCallback(ReadCallback), socket);
            }
            else//未读到数据
            {
                Invoke((new Action(() => //C# 3.0以后代替委托的新方法
                {
                    button1.Text = "连接";
                    textBox3.AppendText("\n异常断开\n");//对话框追加显示数据
                })));
                try
                {
                    MySocket.BeginDisconnect(false, null, null);//断开连接
                }
                catch (Exception) { }
            }
        }

        #region[img]
        private  string GetFullImg(string data)
        {
            int i;
            int jpeg_pos = 0;
            string imgHexStr=null;
            string fullImgHexStr = null;
            data = data.Replace(" ", "");//清除空格
            for (i = 0; i < data.Length; i++)
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
                //写完一个完整包
                if (jpeg_pos >= 4 &&
                    imgHexStr[jpeg_pos - 3] == 'F' && imgHexStr[jpeg_pos - 4] == 'F' &&
                    imgHexStr[jpeg_pos - 1] == '9' && imgHexStr[jpeg_pos - 2] == 'D' &&
                    imgHexStr[0] == 'F' && imgHexStr[1] == 'F' && imgHexStr[2] == 'D' && imgHexStr[3] == '8')
                {
                    fullImgHexStr = imgHexStr;
                    return imgHexStr;
                }

            }
            //return fullImgHexStr;
            return imgHexStr;
        }
        private Image ByteArray2Image(byte[] Bytes)
        {
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                try
                {
                    Image outputImg = Image.FromStream(ms);
                    return outputImg;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Err; " + e.ToString());
                    return pictureBox1.Image;                    
                }
                
            }
        }
        #endregion


        #region[hex]
        /// <字节数组转16进制字符串>
        /// <param name="bytes"></param>
        /// <returns> String 16进制显示形式</returns>
        public static string byteToHexStr(byte[] bytes, int Len)
        {
            string returnStr = "";
            try
            {
                if (bytes != null)
                {
                    for (int i = 0; i < Len; i++)
                    {
                        returnStr += bytes[i].ToString("X2");
                        returnStr += " ";//两个16进制用空格隔开,方便看数据
                    }
                }
                return returnStr;
            }
            catch (Exception)
            {
                return returnStr;
            }
        }

        /// <字符串转16进制格式,不够自动前面补零>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] strToToHexByte(String hexString)
        {
            int i;
            hexString = hexString.Replace(" ", "");//清除空格
            if ((hexString.Length % 2) != 0)//奇数个
            {
                byte[] returnBytes = new byte[(hexString.Length + 1) / 2];
                try
                {
                    for (i = 0; i < (hexString.Length - 1) / 2; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                    returnBytes[returnBytes.Length - 1] = Convert.ToByte(hexString.Substring(hexString.Length - 1, 1).PadLeft(2, '0'), 16);
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
            else
            {
                byte[] returnBytes = new byte[(hexString.Length) / 2];
                try
                {
                    for (i = 0; i < returnBytes.Length; i++)
                    {
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    }
                }
                catch
                {
                    MessageBox.Show("含有非16进制字符", "提示");
                    return null;
                }
                return returnBytes;
            }
        }
        /// <summary>
        /// hex字符串to字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] HexString2Bytes(String hexString)
        {
            hexString = hexString.Replace(" ", "");
            //if (hexString.Length % 2 != 0)
            //{
            //    throw new ArgumentException("参数长度不正确");
            //}

            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }
        #endregion


        #region[Btn]
        /// <清除按钮点击事件>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
        }

        /// <发送按钮点击事件>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            String Str = textBox4.Text.ToString();//获取发送文本框里面的数据
            try
            {
                if (Str.Length > 0)
                {
                    if (checkBox2.Checked)//选择16进制发送
                    {
                        byte[] byteHex = strToToHexByte(Str);
                        MySocket.BeginSend(byteHex, 0, byteHex.Length, 0, null, null); //发送数据
                    }
                    else
                    {
                        byte[] byteArray = Encoding.Default.GetBytes(Str);//Str 转为 Byte值
                        MySocket.BeginSend(byteArray, 0, byteArray.Length, 0, null, null); //发送数据
                    }
                }
            }
            catch (Exception) { }
        }

        /// <清除发送按钮点击事件>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Clear();
        }
        #endregion

    }
}
