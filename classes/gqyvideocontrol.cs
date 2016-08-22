using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Configuration;
using System.Windows;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client;


namespace zsdpmap.classes
{
    public class PointAndDTime
    {
        public MapPoint videodot;
        public DateTime opentime;
    }

    public class gqyvideocontrol
    {
        private const int bufferSize = 8192;

        private static TcpClient myTCPClient = null;

        private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        private static bool IsConnectionSuccessful = false;
        private static Exception socketexception;
        String commandstr;

        public ConcurrentDictionary<string, Object> _videocollection = new ConcurrentDictionary<string, Object>();
        public ConcurrentDictionary<string, Object> _Dynamic_videocollection = new ConcurrentDictionary<string, Object>();


        private static void CallBackMethod(IAsyncResult asyncresult)
        {
 
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;
             
                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                IsConnectionSuccessful = false;
                socketexception = ex;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        public static TcpClient Connect(IPEndPoint remoteEndPoint, int timeoutMSec)
        {
            TimeoutObject.Reset();
            socketexception = null; 

            string serverip = Convert.ToString(remoteEndPoint.Address);
            int serverport = remoteEndPoint.Port;           
            TcpClient tcpclient = new TcpClient();
        
            tcpclient.BeginConnect(serverip, serverport, new AsyncCallback(CallBackMethod), tcpclient);

            if (TimeoutObject.WaitOne(timeoutMSec, false))
            {
                if (IsConnectionSuccessful)
                {
                    return tcpclient;
                }
                else
                {
                    tcpclient.Close();
                    return null;
                }
            }
            else
            {
                tcpclient.Close();
                return null;
                // throw new TimeoutException("TimeOut Exception");
            }
        }


        public TcpClient linkgqy()
        {
            try
            {
                if (myTCPClient != null && !myTCPClient.Connected)  // 如果连接失效清除连接,准备重连
                {
                    myTCPClient.Close();
                    myTCPClient = null;
                }
                if (myTCPClient == null)    // 连接为空重建
                {
                    IPAddress gqyIP = IPAddress.Parse(ConfigurationManager.AppSettings["GQYServerIP"].ToString().Trim());
                    int gqyport = int.Parse(ConfigurationManager.AppSettings["GQYPort"].ToString());
                    IPEndPoint iep = new IPEndPoint(gqyIP, gqyport);

                    myTCPClient = Connect(iep, 500);    // 500ms timeout
                }

 
            }
            catch (Exception)
            {
                if (myTCPClient != null)
                {
                    myTCPClient.Close();
                    myTCPClient = null;
                }
            }
            return myTCPClient;
        }

        public void closegqy()
        {
            return; // GQY 暂时不能支持反复连断

            try
            {
                if (myTCPClient != null)
                    myTCPClient.Close();
                myTCPClient = null;
 
            }
            catch (Exception)
            {
            }
        }



        //以TCP方式发送数据
        public void sendcommandTCP(string gqyvideocommand)
        {

           // NetworkStream clientStream = null;
            try
            {
                if (myTCPClient != null) 
                {
                    Byte[] sendBytes = Encoding.UTF8.GetBytes(gqyvideocommand);
                    myTCPClient.Client.Send(sendBytes);

                }
                /*  不用stream 方法
                    if (clientStream == null)
                        clientStream = myTCPClient.GetStream();
                    if (clientStream.CanWrite)
                    {
                        Byte[] sendBytes = Encoding.UTF8.GetBytes(gqyvideocommand);
                        clientStream.Write(sendBytes, 0, sendBytes.Length);

                    }
                */
 
            }
            catch (Exception)
            {
                if (!myTCPClient.Connected)
                {
                    myTCPClient.Close();
                    myTCPClient = null; 
                }
            }

        }

        public void OpenCARVID(bool v,MapPoint MP)
        {
            try
            {
                MP.SpatialReference = new SpatialReference(4326);
                Point WP = GlobalLayers._MainMap.MapToScreen(MP);
                if (Double.IsNaN(WP.X))
                    return;
                commandstr = "CreateWin_Client,VGA6,";
                commandstr = commandstr + WP.X.ToString() + "," + WP.Y.ToString() + ",";
                commandstr = commandstr + "800,600" + "#";  // 约定以 # 为指令码的结束符
                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }
        public void CloseCARVID()
        {
            try
            {
                commandstr = "DeletWin_Client,VGA6#";

                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        public void OpenVID(String VID, MapPoint MP)
        {


            double OffsetX = 60;    // 图标尺寸，使视频窗避开图标
            double OffsetY = 60;

            try
            {
                MP.SpatialReference = new SpatialReference(4326);
                Point WP = GlobalLayers._MainMap.MapToScreen(MP);
                commandstr = "OpenWin,";

                commandstr = commandstr + VID + ",";
                commandstr = commandstr + (WP.X + OffsetX).ToString() + "," + (WP.Y + OffsetY).ToString() + ",";
                commandstr = commandstr + "800,600" + "#";  // 约定以 # 为指令码的结束符
                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();

                    GlobalLayers._gqyvideocontrol._videocollection.AddOrUpdate(VID, MP, (key, value) => MP);
/*
                    if (_videocollection.ContainsKey(VID))    // 更新视频ID及其位置，用于地图缩放平移后向GQY刷新位置
                        _videocollection.Remove(VID);          // add by john 因为现在open已经改为可重复打开
                    _videocollection.Add(VID, MP); //add by lzd
 */
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



        }
        

        public void CloseVID(String VID)
        {
            string commandstr = "CloseWin," + VID + "#";
            try
            {
                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();
 
             
                 }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                }

        }
/*
        public void FlushStart()
        {
            // 拷贝打开列表
            try
            {
                _TempCollention.Clear();
                foreach (DictionaryEntry obj in _Dynamic_videocollection )
                {
                    _TempCollention.Add(obj.Key, obj.Value);
                }
                _Dynamic_videocollection.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        */

        /*
        public void FlushOpenVID_OLD(string VID,MapPoint MP)
        {
            // 本调用专门用于车辆动态目标的视频刷新显示
            double OffsetX = 60;    // 图标尺寸，使视频窗避开图标
            double OffsetY = 60;

            try
            {
                Point WP = GlobalLayers._MainMap.MapToScreen(MP);
                commandstr = "OpenWin,";

                commandstr = commandstr + VID + ",";
                commandstr = commandstr + (WP.X + OffsetX).ToString() + "," + (WP.Y + OffsetY).ToString() + ",";
                commandstr = commandstr + "800,600" + "#";  // 约定以 # 为指令码的结束符
                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();

                    if (_TempCollention.ContainsKey(VID))
                        _TempCollention.Remove(VID);                // 存在的再次打开的编号表明从列表中去除；为了结束时关闭剩余的视频                  
                    if (_Dynamic_videocollection.ContainsKey(VID))
                        _Dynamic_videocollection.Remove(VID);

                    PointAndDTime ADot = new PointAndDTime();
                    ADot.videodot = MP;
                    ADot.opentime = DateTime.Now;
                    _Dynamic_videocollection.Add(VID, ADot);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        */

        public void FlushOpenVID(string VID, MapPoint MP)
        {
            // 本调用专门用于车辆动态目标的视频刷新显示
            double OffsetX = 60;    // 图标尺寸，使视频窗避开图标
            double OffsetY = 60;

            try
            {
                MP.SpatialReference = new SpatialReference(4326);
                Point WP = GlobalLayers._MainMap.MapToScreen(MP);
                commandstr = "OpenWin,";

                commandstr = commandstr + VID + ",";
                commandstr = commandstr + (WP.X + OffsetX).ToString() + "," + (WP.Y + OffsetY).ToString() + ",";
                commandstr = commandstr + "800,600" + "#";  // 约定以 # 为指令码的结束符
                if (linkgqy() != null)
                {
                    sendcommandTCP(commandstr);
                    closegqy();
                    Object OutObj;
                    _Dynamic_videocollection.TryRemove(VID, out OutObj);

                 /*
                    if (_Dynamic_videocollection.ContainsKey(VID))
                        _Dynamic_videocollection.Remove(VID);
                    */
                    PointAndDTime ADot = new PointAndDTime();
                    ADot.videodot = MP;
                    ADot.opentime = DateTime.Now;
                    _Dynamic_videocollection.AddOrUpdate(VID, ADot, (key, value) => ADot);
                 /*   _Dynamic_videocollection.Add(VID, ADot);*/
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        /*
        public void FlushEnd_OLD()
        {
            Hashtable _tempc = Hashtable.Synchronized(new Hashtable());
            try
            {
                // 关闭没有被刷新过的动态视频
                foreach (DictionaryEntry obj in _TempCollention)
                {
                    TimeSpan diff = DateTime.Now - (obj.Value as PointAndDTime).opentime;
                    if (diff.TotalMinutes > 10) // 10 分钟
                    {
                        CloseVID(obj.Key.ToString());
                    }
                    else
                    {
                        _tempc.Add(obj.Key, obj.Value);
                    }
                }

                _TempCollention.Clear();
                foreach (DictionaryEntry obj in _tempc)
                {
                    _TempCollention.Add(obj.Key, obj.Value);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
 
        }
        */
        public void FlushEnd()
        {
            ConcurrentDictionary<string,Object> _tempc = new ConcurrentDictionary<string,Object>();
            try
            {
                // 关闭没有被刷新过的动态视频
                Graphic GMain = new Graphic();
                GMain.Geometry = new MapPoint(PublicVARS.MainX,PublicVARS.MainY);
                GMain.Geometry.SpatialReference = new SpatialReference(4326);

                foreach (var obj in  _Dynamic_videocollection)
                {
        //            TimeSpan diff = DateTime.Now - (obj.Value as PointAndDTime).opentime;
        //            if (diff.TotalMinutes > 1) // 自动打开的视频1分钟后关闭
                    // 上两行是用打开时间作为关闭条件，在实际使用中效果不好，改为动态跟踪点和打开视频点的距离作为关闭条件，距离在参数VALIDDISTANCE 中设
                    LocalGeometry.DistanceIsValid(GMain,obj.Value as Graphic);
                    {
                        CloseVID(obj.Key.ToString());

                        if (GlobalLayers._GraphicLogCollention.ContainsKey(obj.Key.ToString()))
                        {
                            GlobalLayers._MainMap.Dispatcher.Invoke(
                                new Action(
                                delegate
                                {
                                    GlobalLayers.DynamicResultGraphicLayer.Graphics.Remove(GlobalLayers._GraphicLogCollention[obj.Key.ToString()] as ESRI.ArcGIS.Client.Graphic);

                                }));
                            /*
                            GlobalLayers._GraphicLogCollention.Remove(obj.Key.ToString());
                            */
                            Object OutObj;
                            GlobalLayers._GraphicLogCollention.TryRemove(obj.Key.ToString(), out OutObj);
                        }
                                        
                    
                    }
             //       else
                    {
                        /*
                        _tempc.Add(obj.Key, obj.Value);
                         */
                        _tempc.TryAdd(obj.Key, obj.Value);
                    }
                }

                _Dynamic_videocollection.Clear();
                foreach (var obj in _tempc)
                {
                    _Dynamic_videocollection.TryAdd(obj.Key, obj.Value);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        public void CloseAllFlush()
        {
            foreach (var obj in _Dynamic_videocollection)
            {
                try
                {
                    CloseVID(obj.Key.ToString());
                }
                catch (Exception)
                {
                }
            }
            _Dynamic_videocollection.Clear();
        }

        public void CloseAllVID()
        {
             try
             {
                 foreach (var obj in _videocollection)
                {

                    CloseVID(obj.Key.ToString());
                }
                _videocollection.Clear();
             }

             catch (Exception e)
             {
                 MessageBox.Show(e.ToString());
                }
            

        }

        public void CloseAll()
        {
            try
            {
                if (linkgqy() != null)
                {
                    commandstr = "CloseAllWin#";
                    sendcommandTCP(commandstr);
                    closegqy();
                    _videocollection.Clear();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            
        }
    }
}
