using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;

using zsdpmap.classes;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace zsdpmap
{
    class TcpThread
    {
        public Socket client = null;

        public delegate void NextPrimeDelegate(JObject js);//测试
        CMDOperator CMD;

        public TcpThread(Socket k,  CMDOperator _cmd)
        {
            client = k;
            CMD = _cmd;
        }

        public void SendToClient(string msg)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(msg);
            client.Send(byteArray); // 调试用！回显给客户端

        }
        public void ClientService()
        {
            byte[] bytes = new byte[256*1024];
            string CmdBuffer = null;
            int len;

            //LPY 2015-8-11 22:31:00 添加 每当有新连接，回发一个JSON数组，包含已打开视频列表
            string strJsonOpenedVideoToSend = GetOpenedVideoToSend();
            SendToClient(strJsonOpenedVideoToSend); 
            LogHelper.WriteLog(typeof(MainWindow), strJsonOpenedVideoToSend);

                while (true)
                {
                    try
                    {
                        

                        len = client.Receive(bytes,bytes.Length,0);

                        if (len == 0)       // 客户端已经断开
                            break;

 
                        string str = Encoding.UTF8.GetString(bytes, 0, len);

                        byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(str);
                            //    client.Send(byteArray); // 调试用！回显给客户端


                        while (str.Contains("#"))  // 判断分隔符，如果一个输入缓冲区有多段指令，需要循环拆分出命令
                        {
                            CmdBuffer += str.Substring(0, str.IndexOf("#"));
                            JObject rejson = JObject.Parse(CmdBuffer);

                            if (rejson["CMD"].ToString() == "SYNC")
                            {
                                    // Envelope Evn =;
                                    GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                                        new Action(
                                        delegate
                                        {
                                            MapPoint Center = GlobalLayers._MainMap.Extent.GetCenter();

                                            double x = Center.X;
                                            double y = Center.Y;
                                            int Level = GeometryFunc.GetZoomLevel();
                                            string msg = "{\"CMD\":\"SYNCR\",\"LEVEL\":" + Level.ToString() + "," + "\"X\":" + x.ToString() + "," + "\"Y\":" + y.ToString() + "}";
                                            SendToClient(msg);

                                        }));
                                }
                                else
                                {
                                    GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                                        new Action(
                                            delegate
                                            {
                                                CMD.operatecommand(rejson); // 解析指令的主程序
                                                LogHelper.WriteLog(typeof(MainWindow), rejson["CMD"].ToString());
                                            }));
                                //谢总：测试begininvoke和每个命令用独立线程完成
                                    //Task t = Task.Factory.StartNew(() => {
                                    //    GlobalLayers._MainMap.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new NextPrimeDelegate(CMD.operatecommand), rejson);
                                    //});
                                    //t.Wait();
                                }
                                CmdBuffer = null;

                                str = str.Substring(str.IndexOf("#") + 1, str.Length - str.IndexOf("#") - 1);
                            }
                            CmdBuffer += str;   // 没有收到 # 结尾， 只将收到的信息拼接，继续接收

                        }
                    
                    catch (SocketException SE)
                    {
                        CmdBuffer = null;
                     //   client.Close(); 有这个语句后例外可能会让Socket连接异常后无法退出线程 ？
                        break;
                    }
                    catch (Exception e)
                    {
                       
                        //     MessageBox.Show(e.ToString());
                        CmdBuffer = null;
                 //       break; // 新加，原来版本在2个客户端连接后，CPU使用百分比会急剧提高，判断是这个异常造成
                    }
            }
            client.Close();
 
        }

        private string GetOpenedVideoToSend()
        {
           
            try
            {
                StringBuilder sbToReturn = new StringBuilder();
                StringBuilder sbTemp = new StringBuilder();

                if (GlobalLayers._spinfowindow!=null)
                {
                    GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                                       new Action(
                                       delegate
                                       {
                                           for (int i = 1; i <= 6; i++)//视频底板个数为6，可以用UGRoot.Children.Count代替（给视频底板Uni）
                                           {
                                               TextBlock tb = (TextBlock)GlobalLayers._spinfowindow.FindName("tb" + i.ToString());
                                               if (tb.Tag.ToString() != "0")
                                               { 
                                                   sbTemp.AppendFormat("{{\"attributes\":{{\"ID\":\"{0}\",\"STDID\":\"{1}\",\"MC\":\"{2}\"}}}},", tb.Text.Split('.')[0], tb.Tag,tb.Text.Split('.')[1]); 
                                               }
                                           }
                                           string strTemp = sbTemp.ToString();
                                            if (strTemp.LastIndexOf(",")!=-1)//包含逗号
                                            {
                                                strTemp = strTemp.Remove(strTemp.LastIndexOf(","), 1); 
                                            }
                                            sbToReturn.Append("{\"CMD\":\"SynchronousVideos\",\"videos\":[");
                                            //sbToReturn.Append("{\"attributes\":[");
                                            sbToReturn.Append(strTemp);
                                            sbToReturn.Append("]}]}");
                                       }));
                }
                return sbToReturn.ToString();
            }
            catch (Exception)
            {
                return "";
            }
                                        
        }
    }
}
