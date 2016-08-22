using System;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

using zsdpmap.classes;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Local;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Net;
using ESRI.ArcGIS.Client.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.Globalization;
using System.Configuration;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Threading.Tasks;
using zsdpmap.parts;



namespace zsdpmap
{

   

    public partial class MainWindow : Window
    {

    //    AJ_INFOPAD_SMALL _pinfowindow;

        //定义当前底图类型
     /*   private static string maptype = "0";

        static _GPS gps;
        static AJ aj;
        */

        //定义接收UDP指令线程
   //     private Thread threadudp = null;

        //private static string SL = "http://10.123.7.113:7001/PGIS_S_TileMapServer/Maps/SL/EzMap?Service=getImage&Type=RGB&V=0.3&";
        //private static string YX = "http://10.123.7.113:7001/PGIS_S_TileMapServer/Maps/YX/EzMap?Service=getImage&Type=RGB&V=0.3&";
        //private static string SY = "http://10.123.7.113:7001/PGIS_S_TileMapServer/Maps/SLYX/EzMap?Service=getImage&Type=RGB&V=0.3&";

        private static string SL = ConfigurationManager.AppSettings["SL"];
        private static string YX = ConfigurationManager.AppSettings["YX"];
        private static string SY = ConfigurationManager.AppSettings["SY"];



        int _localport = int.Parse(System.Configuration.ConfigurationManager.AppSettings["LocalPort"].ToString());
        int _DataPort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["DataPort"].ToString());
        int _LicensePort = int.Parse(System.Configuration.ConfigurationManager.AppSettings["LicensePort"].ToString());

        private void ReLoadMe(object sender, ESRI.ArcGIS.Client.Map.MouseEventArgs e)
        {
            Envelope env;
            env = new Envelope();
            env.XMin = 122.06;
            env.YMin = 30.02;
            env.XMax = 122.08;
            env.YMax = 30.04;
            mainmap.Layers["zsstreetmap16"].Visible = false;
            mainmap.Extent = env; //  "121.995787,29.932494,122.256907,30.245483";
            mainmap.Layers["zsstreetmap16"].Visible = true;



        }

        public MainWindow()
        {
            // License setting and ArcGIS Runtime initialization is done in Application.xaml.cs.
            InitializeComponent();
            //VisualStateManager.GoToState(this, "vs1", true);
            #region 设置程序全屏代码
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;

            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            #endregion

            MyTile slmap = new MyTile();
            slmap.Url = SL;
            slmap.ID = "SL";
            slmap.Visible = true;
            slmap.EnableOffline = true;
            slmap.SaveOfflineTiles = true;
            slmap.LoadOfflineTileFirst = true;
            mainmap.Layers.Add(slmap);
            mainmap.Extent = new ESRI.ArcGIS.Client.Geometry.Envelope(121.1, 28.5, 121.38, 28.65);

            MyTile yxmap = new MyTile();
            yxmap.Url = YX;
            yxmap.ID = "YX";
            yxmap.Visible = false;
            yxmap.EnableOffline = true;
            yxmap.SaveOfflineTiles = true;
            yxmap.LoadOfflineTileFirst = true;
            mainmap.Layers.Add(yxmap);
            mainmap.Extent = new ESRI.ArcGIS.Client.Geometry.Envelope(121.1, 28.5, 121.38, 28.65);

            string SKURL = System.Configuration.ConfigurationManager.AppSettings["SHUIKU_PG"];
            ArcGISDynamicMapServiceLayer dy = new ArcGISDynamicMapServiceLayer();
            dy.Opacity = 0.8;
            dy.Url = SKURL;
            mainmap.Layers.Add(dy);

            Thread.CurrentThread.IsBackground = true;
/*
            // 初始化MQ连接
            ConnectMQ();
            if (PUBLIC.connection == null)
                return;
*/
            //初始化当前地图图层
            if (!intGlyr())
            {
                MessageBox.Show("加载地图图层失败", "错误", MessageBoxButton.OK);
            }

        //    Traffic trffic = new Traffic(); // 交通流量



    // 原方案是接收GPS数据先保存在缓冲区，启动MoveingTarget 线程循环绘制
     //       MovingTarget ProcessCars = new MovingTarget();
            

            // UDP 方式接受GPS 数据改为 MQ 订阅
     //       PublicVARS.threadDataudp = new Thread(new ThreadStart(ReceiveData2));   // 数据流接收 从 2301，UDP
     //       PublicVARS.threadDataudp.Start();

           
            ScanMainCar SMC = new ScanMainCar();   // 指定对象的跟踪处理
       //     Thread ScanMain = new Thread(SMC.AsyncLoop);
            Task ScanMain = new Task(SMC.AsyncLoop);    // 主控车跟踪
            ScanMain.Start();





         //   mainmap.Layers["Traffic"].Visible = false;
        //    ArcGISLocalDynamicMapServiceLayer Traffic = (ArcGISLocalDynamicMapServiceLayer)mainmap.Layers["Traffic"];


            // 接受Flex 客户端控制
       //     Thread License = new Thread(new ThreadStart(ReceiveLicenseTCP));
            Task License = new Task(ReceiveLicenseTCP);
            License.Start();



            if (GlobalLayers._policepointglr == null)
            {
                GlobalLayers._policepointglr = new GraphicsLayer();
                GlobalLayers._policepointglr.ID = "policepoint";
                GlobalLayers._policepointglr.DisplayName = "动态目标";
                mainmap.Layers.Add(GlobalLayers._policepointglr);

            }
            // Gps 处理主线程
       //     Thread GpsProcessThread = new Thread(new ThreadStart(ProcessGPS));
            Task GpsProcessThread = new Task(ProcessGPS);
            GpsProcessThread.Start();

            //LPY 2014-12-25 10:56:32 添加 水库数据处理主线程
            //Thread SKThread = new Thread(new ThreadStart(ShowSKInfoWindow));
            //SKThread.Start();
            
            try
            {
                GlobalLayers.ActiveLayer = new GraphicsLayer();
                GlobalLayers.ActiveLayer.ID = "AJ_ACTIVE";
                mainmap.Layers.Add(GlobalLayers.ActiveLayer);
            //    mainmap.Layers.Insert()
            }
            catch (Exception)
            {
                GlobalLayers.ActiveLayer = null;
            };
            
            // 案件信息窗
            OpenAJINFO();

            //水库窗口--暂时注释
            //if (ConfigurationManager.AppSettings["IsShow"]=="1")
            //{
            OpenSKINFO();
            //}

            //LPY 2015-1-21 22:15:34 添加 视频窗口列表底图
            OpenSPINFO(); OpenSPLDPAD();//视频联动专用底板

            //LPY 2015-3-14 12:52:32 添加 信息窗口
            OpenXXINFO();

            //LPY 2015-3-26 14:51:35 添加 警车信息窗口
            OpenCarInfo();

            //LPY 2015-4-1 13:49:00 添加 预案信息窗口
            OpenYAInfo();

            //LPY 2015-5-22 18:50:13 添加 地图上方LOGO
            OpenLogoInfo();

            //LPY 2015-5-27 12:33:52 添加 通话窗口
            OpenTelInfo();

            //LPY 2015-7-9 14:22:49 添加 显示HTML文件的信息窗口
            OpenYAHtmlPAD();

            //LPY 2015-8-11 19:46:44 添加 预案巡逻线路信息显示
            OpenYAINFOPAD();

            CreatePadMenu();
            // 接收客户端控制代码
         //   PublicVARS.threadtcp = new Thread(new ThreadStart(ReceiveCmdTCP));        // 控制指令接收 从 2300，TCP
            PublicVARS.threadtcp = new Task(ReceiveCmdTCP);
            PublicVARS.threadtcp.Start();

            //LPY 2015-1-24 18:34:20 添加 初始化SDK
            if (ConfigurationManager.AppSettings["debug"]=="1")
            {
                try
                {
                    string lyuser = ConfigurationManager.AppSettings["LYUser"];
                    string lypwd = ConfigurationManager.AppSettings["LYPWD"];
                    string lyip = ConfigurationManager.AppSettings["LYIP"];
                    string lyport = ConfigurationManager.AppSettings["LYPort"];
                    string b20user = ConfigurationManager.AppSettings["B20User"];
                    string b20pwd = ConfigurationManager.AppSettings["B20PWD"];
                    string b20ip = ConfigurationManager.AppSettings["B20IP"];
                    string b20port = ConfigurationManager.AppSettings["B20Port"];

                    //DllInvoke dll = new DllInvoke(@"NVideo.dll");

                    VideoToWall._initnv(lyuser, lypwd, lyip, Convert.ToInt32(lyport));
                    VideoToWall._initb20(b20user, b20pwd, b20ip, Convert.ToInt32(b20port));

                    for (int i = 0; i < CMDOperator._b20controldata.Length; i++)
                    {
                        CMDOperator._b20controldata[i].ptr = Marshal.AllocHGlobal(128);
                        CMDOperator._b20controldata[i].DB33 = "";                         
                    }

                    for (int i = 0; i < CMDOperator._b20controldataSPLD.Length; i++)
                    {
                        CMDOperator._b20controldataSPLD[i].ptr = Marshal.AllocHGlobal(128);
                        CMDOperator._b20controldataSPLD[i].DB33 = "";
                    }


                    CMDOperator._b20controldataZoomUp.ptr = Marshal.AllocHGlobal(128);
                    CMDOperator._b20controldataZoomUp.DB33 = "";



                    QueryLayer._currentVideo.ptr = Marshal.AllocHGlobal(128); 
                    QueryLayer._currentVideo.DB33 = "";
                }
                catch (Exception)
                {
                }
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    Label lb = new Label();
            //    lb.Name = "lb" + i.ToString();
            //    lb.ContentTemplate = LayoutRoot.Resources["ButtonTemplateSpecial"] as DataTemplate;
            //    lb.Content = JObject.Parse("{Title:\"院桥" + i.ToString() + "\"}");
            //    lb.Margin = new Thickness(i * 100, 0, 0, 0);
            //    lb.VerticalAlignment = VerticalAlignment.Bottom;
            //    LayoutRoot.Children.Add(lb);
            //}
            
        }

        private void ProcessAJ()
        {
       //     OpenAJINFO();
            while (true)
            {
                Thread.Sleep(3);
            }
        }

        private void ProcessGPS()
        {
            _GPS gps = new _GPS();
            while (true)
            {
                Thread.Sleep(30);
            }
        }

        //LPY 2014-12-25 10:54:12 添加 线程监听MQ发来的水库数据
        private void ShowSKInfoWindow()
        {
            
            SKInfo skInfo = new SKInfo();
        }

        #region TCP 接收指令
        private void ReceiveCmdTCP()
        {
            IPAddress local = IPAddress.Any;
            IPEndPoint iep = new IPEndPoint(local, _localport);
          
            Socket server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp );
            server.Bind(iep);
            server.Listen(50);
       //     CMDOperator CMD = new CMDOperator(mainmap, LayoutRoot, this);



            while (true)    // 等待TCP连接
            {
                try
                {
                    Socket client = server.Accept();        // 同步方式等待客户端接入
                    CMDOperator CMD = new CMDOperator(this);
                    TcpThread newClient = new TcpThread(client, CMD);
               //     Thread newThread = new Thread(new ThreadStart(newClient.ClientService));    // 为每个连接单独建一个线程处理命令
                    Task newThread = new Task(newClient.ClientService);
                    newThread.Start();
                }
                catch (Exception)
                {

                }
            }

        }
        #endregion

  

        #region TCP License for Flex

        private void ReceiveLicenseTCP()
        {
            string crossdomain = "<?xml version=\"1.0\"?><cross-domain-policy><site-control permitted-cross-domain-policies=\"all\"/><allow-access-from domain=\"*\" to-ports=\"*\"/></cross-domain-policy>\0";
            string policy = "<policy-file-request/>";

            TcpListener listener = new TcpListener(IPAddress.Any,843);
            listener.Start();

            while(true)
            {
                try
                {
                    const int bufferSize = 204800;
                    TcpClient client = listener.AcceptTcpClient();
                    NetworkStream clientStream = client.GetStream();
                    byte[] buffer = new byte[bufferSize];
                    int readBytes = 0;
                    readBytes = clientStream.Read(buffer, 0, bufferSize);
                    string request = Encoding.ASCII.GetString(buffer).Substring(0, readBytes);
                    if (request.IndexOf(policy) != -1)
                    {
                        byte[] stringoutBuffer = Encoding.ASCII.GetBytes(crossdomain);
                        clientStream.Write(stringoutBuffer, 0, stringoutBuffer.Length);                            
                    }
                    clientStream.Close();
                    Thread.Sleep(10);
                }
                catch(Exception)
                {

                }
            }
/*
            IPAddress local = IPAddress.Any;
            IPEndPoint iep = new IPEndPoint(local, _LicensePort);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(50);
            int len = 0;
            byte[] bytes = new byte[1024];
            while (true)    // 等待TCP连接
            {
                Socket client = server.Accept();
                try
                {
                    if (client == null)
                        continue;
                    while ((len = client.Receive(bytes)) != 0)
                    {
                        string str = Encoding.UTF8.GetString(bytes, 0, len);
                        if (str == policy)
                        {
                            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(crossdomain);
                            client.Send(byteArray); // 回显给请求端
                        }

                        Thread.Sleep(3);
                    }
                    client.Close();

                }
                catch (Exception)
                {
                }
            
            }
 * */
        }
        #endregion
/*
        #region UDP接收指令
        private void ReceiveCmd()
        {

            
   
            


            try
            {
                CMDOperator CMD = new CMDOperator(mainmap, GG, LayoutRoot, this);
                UdpClient _udpClient = new UdpClient(_localport);   // 打开命令接收端口          
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            }
            catch
            {

            }
            //接收从控制机发送过来的控制命令；
            while (true)
            {
                try
                {
                    byte[] bytes = _udpClient.Receive(ref remote);
                    string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    JObject rejson = JObject.Parse(str);
                    this.mainmap.Dispatcher.Invoke(
                        new Action(
                             delegate
                             {
                                 CMD.operatecommand(rejson);
                             }
                                )
                        );  
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                    //break;
                }
                Thread.Sleep(10);
            }
        }
*/
        /*
        private void ReceiveData()
        {

            UdpClient _udpClient = new UdpClient(_DataPort);

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            CMDOperator CMD = new CMDOperator(this);

            //接收从远程主机发送过来的信息；

            while (true)
            {
            try
            {
                    byte[] bytes = _udpClient.Receive(ref remote);
                    if (bytes.Length > 0)
                    {
                        string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        JObject rejson = JObject.Parse(str);

                        CMD.operatecommand(rejson);

                    }
                }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //break;
            }
            }

        }

        private void ReceiveData2()
        {

            UdpClient _udpClient = new UdpClient(_DataPort);

            IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
            CMDOperator CMD = new CMDOperator(this);

            //接收从远程主机发送过来的信息；

            while (true)
            {
                try
                {
                    byte[] bytes = _udpClient.Receive(ref remote);
                    if (bytes.Length > 0)
                    {
                        string str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        JObject rejson = JObject.Parse(str);
                       
           //             CMD.operatecommand(rejson);
                        if (rejson["CMD"].ToString() == "00080")
                        {
                            string Key = rejson["HH"].ToString();   // GPS ID 
                            PublicVARS.MovingTargetPool.AddOrUpdate(Key, rejson, (K,V) => rejson);
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //break;
                }
            }

        }

        */

       


        void ConnectMQ()
        {
            /*
            Task connectMQ = new Task(() =>
            {

                try
                {
                    IConnectionFactory factory;

                    factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());   // 连接MQ数据发布服务器
                    PUBLIC.connection = factory.CreateConnection();

                    PUBLIC.connection.Start();
                }
                catch (Exception)
                {
                    PUBLIC.connection = null;
                   // MessageBox.Show("MQ连接失败" + e.ToString());

                }
            });
            connectMQ.Start();
  */
        }
        #region 初始化地图上的图层
        public bool intGlyr()
        {     
            
            try
            {
                if (GlobalLayers.TrafficLayer == null)
                {
                    GlobalLayers.TrafficLayer = new FeatureLayer();
                    GlobalLayers.TrafficLayer.ID = "ROAD";
                    GlobalLayers.TrafficLayer.Url = System.Configuration.ConfigurationManager.AppSettings["ROAD"].ToString();
                    mainmap.Layers.Add(GlobalLayers.TrafficLayer);
                    GlobalLayers.TrafficLayer.Visible = false;
                }
            }
            catch (Exception)
            {
                GlobalLayers.TrafficLayer = null;
            //    MessageBox.Show("道路数据初始化失败，检查地图数据服务");

            }

            try
            {
                if (GlobalLayers.LightsLayer == null)
                {
                    GlobalLayers.LightsLayer = new FeatureLayer();
                    GlobalLayers.LightsLayer.ID = "LIGHTS";
                    GlobalLayers.LightsLayer.Url = System.Configuration.ConfigurationManager.AppSettings["LIGHTS"].ToString();
                    mainmap.Layers.Add(GlobalLayers.LightsLayer);
                    GlobalLayers.LightsLayer.Visible = false;
                }
            }
            catch (Exception)
            {
                GlobalLayers.LightsLayer = null;
              //  MessageBox.Show("信号灯数据初始化失败，检查地图数据服务");
            }

            
            try
            {


                if (GlobalLayers.canvasLayer == null) // 绘制多边形，线，等GEO对象的图层
                {
                    GlobalLayers.canvasLayer = new GraphicsLayer();
                    GlobalLayers.canvasLayer.ID = "canvasLayer";
                    GlobalLayers.canvasLayer.DisplayName = "canvas";
                    mainmap.Layers.Add(GlobalLayers.canvasLayer);
                }

                if (GlobalLayers.BufferGraphicLayer == null)
                {
                    GlobalLayers.BufferGraphicLayer = new GraphicsLayer();
                    GlobalLayers.BufferGraphicLayer.ID = "BufferGraphicsLayer";
                    GlobalLayers.BufferGraphicLayer.DisplayName = "Buffer";
                    mainmap.Layers.Add(GlobalLayers.BufferGraphicLayer);
                }
                if (GlobalLayers.ResultGraphicLayer == null)
                {
                    GlobalLayers.ResultGraphicLayer = new GraphicsLayer();
                    GlobalLayers.ResultGraphicLayer.ID = "ResultGraphicLayer";
                    GlobalLayers.ResultGraphicLayer.DisplayName = "Result";
                    mainmap.Layers.Add(GlobalLayers.ResultGraphicLayer);
                }

                if (GlobalLayers._crimepointglr == null)
                {
                    GlobalLayers._crimepointglr = new GraphicsLayer();
                    GlobalLayers._crimepointglr.ID = "crimepoint";
                    GlobalLayers._crimepointglr.DisplayName = "警情点";
                    mainmap.Layers.Add(GlobalLayers._crimepointglr);
                }



                if (GlobalLayers._offlineglr == null) // 离线车图层
                {
                    GlobalLayers._offlineglr = new GraphicsLayer();
                    GlobalLayers._offlineglr.ID = "offlinepoint";
                    GlobalLayers._offlineglr.DisplayName = "离线";
                    GlobalLayers._offlineglr.Visible = true; // 默认显示
                    mainmap.Layers.Add(GlobalLayers._offlineglr);
                }

                if (GlobalLayers._linklineglr == null)
                {
                    GlobalLayers._linklineglr = new GraphicsLayer();
                    GlobalLayers._linklineglr.ID = "linkline";
                    GlobalLayers._linklineglr.DisplayName = "关联线";
                    mainmap.Layers.Add(GlobalLayers._linklineglr);
                }

                if (GlobalLayers.DynamicResultGraphicLayer == null) //  
                {
                    GlobalLayers.DynamicResultGraphicLayer = new GraphicsLayer();
                    GlobalLayers.DynamicResultGraphicLayer.ID = "DynamicResultGraphicLayer";
                    GlobalLayers.DynamicResultGraphicLayer.DisplayName = "DYNARES";
                    mainmap.Layers.Add(GlobalLayers.DynamicResultGraphicLayer);
                }
                if (GlobalLayers.DynamicGraphicLayer == null) //  
                {
                    GlobalLayers.DynamicGraphicLayer = new GraphicsLayer();
                    GlobalLayers.DynamicGraphicLayer.ID = "DynamicGraphicLayer";
                    GlobalLayers.DynamicGraphicLayer.DisplayName = "DYNA";
                    mainmap.Layers.Add(GlobalLayers.DynamicGraphicLayer);

                }
                if (GlobalLayers.FocusLayer == null) //  
                {
                    GlobalLayers.FocusLayer = new GraphicsLayer();
                    GlobalLayers.FocusLayer.ID = "FocusLayer";
                    GlobalLayers.FocusLayer.DisplayName = "FOCUS";
                    mainmap.Layers.Add(GlobalLayers.FocusLayer);
                    
                }
                
                try
                {
                    GlobalLayers.TodayAJ = new FeatureLayer();
                    GlobalLayers.TodayAJ.ID = "TODAYAJ";
                    GlobalLayers.TodayAJ.Url = ConfigurationManager.AppSettings["TODAYAJ"].ToString();
                    //LPY 2014-12-15 16:09:28 添加 案件过滤条件
                    GlobalLayers.TodayAJ.Where = "bjsj >= date '"+DateTime.Now.ToString("yyyy-MM-dd")+"'";
                    GlobalLayers.TodayAJ.OutFields = new OutFields { "*" };
                    mainmap.Layers.Add(GlobalLayers.TodayAJ);
                    //LPY 2015-6-7 23:49:06 修改 默认隐藏当日案件的显示
                    //GlobalLayers.TodayAJ.Visible = false;
                }
                catch(Exception)
                {
                    GlobalLayers.TodayAJ = null;
                }

                try
                {

                    //string MF_BZD_URL = ConfigurationManager.AppSettings["MF_BZD"];
                    //GlobalLayers.mfbzdlyr = new FeatureLayer();
                    //GlobalLayers.mfbzdlyr.Url = MF_BZD_URL;
                    //GlobalLayers.mfbzdlyr.ID = "MF_BZD";
                    //GlobalLayers.mfbzdlyr.Opacity = 0.9;
                    //GlobalLayers.mfbzdlyr.Visible = false;
                    //mainmap.Layers.Add(GlobalLayers.mfbzdlyr);

                    string MF_BZD_URL = ConfigurationManager.AppSettings["MF_BZD"];
                    //GlobalLayers.mfbzdlyr = new ArcGISDynamicMapServiceLayer();
                    //GlobalLayers.mfbzdlyr.Opacity = 1;
                    //GlobalLayers.mfbzdlyr.Url = MF_BZD_URL;
                    //GlobalLayers.mfbzdlyr.Visible = false;
                    GlobalLayers.mfbzdlyr = new FeatureLayer() { ID = "MF_BZD", Url = MF_BZD_URL, Renderer = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForBZD"] }, Where = " 1=1 ", Visible = false };
                    GlobalLayers.mfbzdlyr.OutFields.Add("*");
                    mainmap.Layers.Add(GlobalLayers.mfbzdlyr);

                }
                catch (Exception)
                {
                    GlobalLayers.mfbzdlyr = null;
                    LogHelper.WriteLog(typeof(MainWindow), "避灾点图层加载失败！");
                }

                try//LPY 2015-8-12 08:09:06 添加 民防避灾点疏散线路图层改为在这里加载
                {
                    string MF_PG_URL = ConfigurationManager.AppSettings["MF_PG"];
                    string LINE_PL_URL = ConfigurationManager.AppSettings["LINE_PL"];                        
                    double opacity = Convert.ToDouble(ConfigurationManager.AppSettings["opacity"]);

                    FeatureLayer mfpglyr = new FeatureLayer();
                    mfpglyr.Url = MF_PG_URL;
                    mfpglyr.ID = "MF_PG";
                    mfpglyr.Opacity = opacity;
                    mfpglyr.Visible = false;
                    mainmap.Layers.Add(mfpglyr);

                    FeatureLayer linepllyr = new FeatureLayer();
                    linepllyr.Url = LINE_PL_URL;
                    linepllyr.ID = "LINE_PL";
                    linepllyr.Opacity = opacity;
                    linepllyr.Visible = false;
                    mainmap.Layers.Add(linepllyr);

                    GlobalLayers.glMFBZDLine1.ID = "glMFBZDLine1";mainmap.Layers.Add(GlobalLayers.glMFBZDLine1);
                    GlobalLayers.glMFBZDLine2.ID = "glMFBZDLine2"; mainmap.Layers.Add(GlobalLayers.glMFBZDLine2);
                    GlobalLayers.glMFBZDLine3.ID = "glMFBZDLine3"; mainmap.Layers.Add(GlobalLayers.glMFBZDLine3);
                    GlobalLayers.glShelter.ID = "glShelter"; mainmap.Layers.Add(GlobalLayers.glShelter); GlobalLayers.glShelter.Visible = false;

                    DrawShelter(121.26906, 28.537926, "占堂");
                    DrawShelter(121.196993, 28.529163, "岙里街村");
                    DrawShelter(121.261557, 28.575069, "繁荣村");

                    GlobalLayers.glPlot1.ID = "glPlot1"; mainmap.Layers.Add(GlobalLayers.glPlot1); GlobalLayers.glPlot1.Visible = false;
                    GlobalLayers.glPlot2.ID = "glPlot2"; mainmap.Layers.Add(GlobalLayers.glPlot2); GlobalLayers.glPlot2.Visible = false;
                    GlobalLayers.glPlot3.ID = "glPlot3"; mainmap.Layers.Add(GlobalLayers.glPlot3); GlobalLayers.glPlot3.Visible = false;

                    DrawPlotByStr(GlobalLayers.Base64line1, "glPlot1");
                    DrawPlotByStr(GlobalLayers.Base64line2, "glPlot2");
                    DrawPlotByStr(GlobalLayers.Base64line3, "glPlot3"); 
                }
                catch (Exception)
                {
                }

                //LPY 2015-5-27 14:59:19 添加 GraphicsLayer 用来存放上墙视频标签
                try
                {
                    if (GlobalLayers.graphicsLayerSPLabel==null)
                    {
                        GlobalLayers.graphicsLayerSPLabel = new GraphicsLayer();
                        GlobalLayers.graphicsLayerSPLabel.ID = "graphicsLayerSPLabel";
                        GlobalLayers.graphicsLayerSPLabel.DisplayName = "SPLabel";                        
                        mainmap.Layers.Add(GlobalLayers.graphicsLayerSPLabel);
                    }
                }
                catch (Exception)
                {
                    
                }
                

                //if(_videoFlyr==null)
                //{
                //    _videoFlyr = new FeatureLayer();
                //    _videoFlyr.Url = "http://localhost:6080/arcgis/rest/services/zydata/MapServer/0";
                //    _videoFlyr.Where = "1=1";
                //    _videoFlyr.OutFields.Add("MC");
                //    SimpleRenderer render = new SimpleRenderer();
                //    render.Symbol = (Symbol)this.Resources["video"];
                //    _videoFlyr.Renderer = render;
                //    mainmap.Layers.Add(_videoFlyr);
                //}

                GlobalLayers._MainMap = mainmap;
                GlobalLayers.LayoutRoot = LayoutRoot;
                GlobalLayers.InfoWin = InfoWin;
                GlobalLayers.InfoWinMF = InfoWinMF; 

                //LPY 2014-12-25 10:43:35 添加 水库InfoWindow
                //GlobalLayers.iwSK1 = iwSK1;
/*
                if (GlobalLayers._policepointglr == null)
                {
                    GlobalLayers._policepointglr = new GraphicsLayer();
                    GlobalLayers._policepointglr.ID = "policepoint";
                    GlobalLayers._policepointglr.DisplayName = "动态目标";
                    mainmap.Layers.Add(GlobalLayers._policepointglr);

                }
 */ 
             //   int idx = mainmap.Layers.IndexOf(GlobalLayers._policepointglr);
             //   mainmap.Layers.Move(idx, 99);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        //图层打开
        private void button6_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalLayers._mapcontrol.OpenGlyr("linkline",mainmap))
            {
            };
        }

        //图层关闭
        private void button7_Click(object sender, RoutedEventArgs e)
        {
            if (!GlobalLayers._mapcontrol.CloseGlyr("linkline", mainmap))
            {
            };
        }

        #region 信息开窗
        private void OpenAJINFO()
        {
        //    AJ_INFOPAD_SMALL _pinfowindow;
            AJ_INFOPAD _pinfowindow;
            
            GlobalLayers._pinfowindow = new AJ_INFOPAD();
            _pinfowindow = GlobalLayers._pinfowindow;
            _pinfowindow.Margin = new Thickness(20);
            _pinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _pinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _pinfowindow.Width = 1;
            _pinfowindow.Height = 1;
       //     _pinfowindow.wbs.Navigate("http://www.sina.com.cn");
      //      _pinfowindow.lbsTitle.Content = "dafadfadfads";
            _pinfowindow.Name = "ANJIAN";
   
            LayoutRoot.Children.Add(_pinfowindow);
         //   _pinfowindow.Show();
        }
        private void DrawPlotByStr(string base64,string glName)
        {
            try
            {
                Graphic g = new Graphic()
                {
                    Geometry = ESRI.ArcGIS.Client.Geometry.Geometry.FromJson(JObject.Parse(Encoding.Default.GetString(Convert.FromBase64String(base64)))["Geometry"].ToString()),
                    Symbol = App.Current.Resources["DrawFillSymbol"] as Symbol
                };
                GraphicsLayer glShelter = mainmap.Layers[glName] as GraphicsLayer;
                //LogHelper.WriteLog(typeof(MainWindow), glShelter == null ? "null" : "not null");
                glShelter.Graphics.Add(g);
                //LogHelper.WriteLog(typeof(MainWindow), glName+"箭头添加成功！");
            }
            catch (Exception ex)
            {
                //LogHelper.WriteLog(typeof(MainWindow), glName+"箭头添加失败:"+ex.Message);                
            }
            
        }
        private void DrawShelter(double x, double y, string name)
        {
            Graphic graphic = new Graphic()
            {
                Geometry = new MapPoint(x, y, new SpatialReference(4326)),
                Symbol = App.Current.Resources["MSShelter"] as Symbol
            };
            graphic.Attributes["MC"] = name;

            GraphicsLayer glShelter = mainmap.Layers["glShelter"] as GraphicsLayer;
            glShelter.Graphics.Add(graphic);
        }
        private void OpenSKINFO()
        {
            SK_INFOPAD _skinfowindow;
            GlobalLayers._skinfowindow = new SK_INFOPAD();
            _skinfowindow = GlobalLayers._skinfowindow;
            _skinfowindow.Margin = new Thickness(0,0,0,0);//1700,200
            _skinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _skinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _skinfowindow.Width = 1;
            _skinfowindow.Height = 1;
            _skinfowindow.Name = "SKYL";

            LayoutRoot.Children.Add(_skinfowindow);
        }

        private void OpenSPINFO()
        {
            SP_INFOPAD _spinfowindow;
            GlobalLayers._spinfowindow = new SP_INFOPAD();
            _spinfowindow = GlobalLayers._spinfowindow;
            _spinfowindow.Margin = new Thickness(0);
            _spinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _spinfowindow.HorizontalAlignment = HorizontalAlignment.Right;
            _spinfowindow.Width = 1;
            _spinfowindow.Height = 1;
            _spinfowindow.tb1.Tag = "0";
            _spinfowindow.tb2.Tag = "0";
            _spinfowindow.tb3.Tag = "0";
            _spinfowindow.tb4.Tag = "0";
            _spinfowindow.tb5.Tag = "0";
            _spinfowindow.tb6.Tag = "0";
            _spinfowindow.tb1.Opacity = 0;
            _spinfowindow.tb2.Opacity = 0;
            _spinfowindow.tb3.Opacity = 0;
            _spinfowindow.tb4.Opacity = 0;
            _spinfowindow.tb5.Opacity = 0;
            _spinfowindow.tb6.Opacity = 0;
            GlobalLayers.LayoutRoot.Children.Add(_spinfowindow);
        }

        private void OpenXXINFO()
        {
            XX_INFOPAD _xxinfowindow;
            GlobalLayers._xxinfowindow = new XX_INFOPAD();
            _xxinfowindow = GlobalLayers._xxinfowindow;
            _xxinfowindow.Margin = new Thickness(0, 0, 0, 0);//Thickness(1700, 200, 0, 0);
            _xxinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _xxinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _xxinfowindow.Width = 1;
            _xxinfowindow.Height = 1;
            _xxinfowindow.Name = "XXINFO";
            LayoutRoot.Children.Add(_xxinfowindow);
        }

        private void OpenCarInfo()
        {
            CAR_INFOPAD _carinfowindow;
            GlobalLayers._carinfowindow = new CAR_INFOPAD();
            _carinfowindow = GlobalLayers._carinfowindow;
            _carinfowindow.Margin = new Thickness(0);
            _carinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _carinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _carinfowindow.Width = 1;
            _carinfowindow.Height = 1;
            _carinfowindow.Name = "CARINFO";
            LayoutRoot.Children.Add(_carinfowindow);
        }

        private void OpenYAInfo()
        {
            YA_INFOPAD _yainfowindow;
            GlobalLayers._yainfowindow = new YA_INFOPAD();
            _yainfowindow = GlobalLayers._yainfowindow;
            _yainfowindow.Margin = new Thickness(0,0,0,0);//1700,200,0,0
            _yainfowindow.VerticalAlignment = VerticalAlignment.Top;
            _yainfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _yainfowindow.Width = 1;
            _yainfowindow.Height = 1;
            _yainfowindow.Name = "YAINFO";
            LayoutRoot.Children.Add(_yainfowindow);
        }

        private void OpenLogoInfo()
        {
            Logo_INFOPAD _logoinfowindow;
            GlobalLayers._logoinfowindow = new Logo_INFOPAD();
            _logoinfowindow = GlobalLayers._logoinfowindow;
            _logoinfowindow.Margin = new Thickness(1600, 0, 0, 0);
            _logoinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _logoinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _logoinfowindow.Width = 3100;
            _logoinfowindow.Height = 200;
            _logoinfowindow.Name = "LogoINFO";
            LayoutRoot.Children.Add(_logoinfowindow);            
        }

        //LPY 2015-5-26 15:39:43 添加 电话连线窗口
        private void OpenTelInfo()
        {
            Tel_INFOPAD _telinfowindow;
            GlobalLayers._telinfowidow = new Tel_INFOPAD();
            _telinfowindow = GlobalLayers._telinfowidow;
            _telinfowindow.Width = 1;
            _telinfowindow.Height = 1;
            _telinfowindow.Margin = new Thickness(3250, 0, 0, 0);
            _telinfowindow.VerticalAlignment = VerticalAlignment.Top;
            _telinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
            _telinfowindow.Name = "TelINFO";
            LayoutRoot.Children.Add(_telinfowindow);

        }

        private void OpenSPLDPAD()
        {
            SP_PADSPLD _sppadSPLD;
            GlobalLayers._sppadSPLD = new SP_PADSPLD();
            _sppadSPLD = GlobalLayers._sppadSPLD;
            _sppadSPLD.Margin = new Thickness(0);
            _sppadSPLD.VerticalAlignment = VerticalAlignment.Top;
            _sppadSPLD.HorizontalAlignment = HorizontalAlignment.Right;
            _sppadSPLD.Width = 1;
            _sppadSPLD.Height = 1;
            _sppadSPLD.tb1.Tag = "0";
            _sppadSPLD.tb2.Tag = "0";
            _sppadSPLD.tb3.Tag = "0";
            _sppadSPLD.tb4.Tag = "0";
            _sppadSPLD.tb5.Tag = "0";
            _sppadSPLD.tb6.Tag = "0";
            _sppadSPLD.tb1.Opacity = 0;
            _sppadSPLD.tb2.Opacity = 0;
            _sppadSPLD.tb3.Opacity = 0;
            _sppadSPLD.tb4.Opacity = 0;
            _sppadSPLD.tb5.Opacity = 0;
            _sppadSPLD.tb6.Opacity = 0;
            GlobalLayers.LayoutRoot.Children.Add(_sppadSPLD);
        }

        private void OpenYAHtmlPAD()
        {
            YA_HTMLPad _yahtmlpad;
            GlobalLayers._yahtmlpad = new YA_HTMLPad();
            _yahtmlpad = GlobalLayers._yahtmlpad;
            _yahtmlpad.Margin = new Thickness(0,0,0,0);//1700,200,0,0
            _yahtmlpad.VerticalAlignment = VerticalAlignment.Top;
            _yahtmlpad.HorizontalAlignment = HorizontalAlignment.Left;
            _yahtmlpad.Width = 1;
            _yahtmlpad.Height = 1;
            GlobalLayers.LayoutRoot.Children.Add(_yahtmlpad);
        }

        private void OpenYAINFOPAD()
        {
            YA_ROADINFOPAD _yaroadinfopad;
            GlobalLayers._yaroadinfopad = new YA_ROADINFOPAD();
            _yaroadinfopad = GlobalLayers._yaroadinfopad;
            _yaroadinfopad.Margin = new Thickness(0);
            _yaroadinfopad.VerticalAlignment = VerticalAlignment.Top;
            _yaroadinfopad.HorizontalAlignment = HorizontalAlignment.Left;
            _yaroadinfopad.Width = 1;
            _yaroadinfopad.Height = 1;
            GlobalLayers.LayoutRoot.Children.Add(_yaroadinfopad);
        }

        //LPY 2015-12-15 10:32:45 添加 菜单PAD
        private void CreatePadMenu()
        {
            PadMenu padMenu = new PadMenu() { Width = 6400, Height = 2400, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 0), Name = "padMenu", Opacity = 0 };
            GlobalLayers.padMenu = padMenu;
            GlobalLayers.LayoutRoot.Children.Add(padMenu);
        }
        #endregion



        #region 信息关窗
        private void button9_Click(object sender, RoutedEventArgs e)
        {
            //if (_pinfowindow!=null)
            //{
            //    LayoutRoot.Children.Remove(_pinfowindow);
            //}

            //mapwindows ui = this.FindName("aaa") as mapwindows;
            LayoutRoot.Children.Remove(LayoutRoot.FindName("aaa") as UIElement);
        }
        #endregion 

        private void button10_Click(object sender, RoutedEventArgs e)
        {
            //GlobalLayers._HeatMapLayercontrol.AddHeatMapLayer(mainmap);
        }

        #region  根据指令控制局部地图窗口
        private string createmapwindow(JObject jsoncommand)
        {
            try
            {
                string pName = jsoncommand["WINID"].ToString();

                if (LayoutRoot.FindName(pName) == null)
                {
                    mapwindows mapwindow = new mapwindows();
                    MapPoint pPoint = new MapPoint((double)jsoncommand["CENTX"], (double)jsoncommand["CENTY"]);
                    double pointx = (double)jsoncommand["CENTX"];
                    double pointy = (double)jsoncommand["CENTY"];
                    mapwindow.map.Extent = new Envelope(pointx - 0.0000035691915087454205 * 800, pointy - 0.0000035691915087454205 * 600, pointx + 0.0000035691915087454205 * 800, pointy + 0.0000035691915087454205 * 600);

                    mapwindow.VerticalAlignment = VerticalAlignment.Top;
                    mapwindow.HorizontalAlignment = HorizontalAlignment.Left;
                    mapwindow.Width = 800;
                    mapwindow.Height = 600;
                    mapwindow.Margin = new Thickness(600, 200, 0, 0);
                    mapwindow.Name = pName;
                    mapwindow.lbsTitle.Content = pName;
                    this.RegisterName(pName, mapwindow);
                    LayoutRoot.Children.Add(mapwindow);
                    return pName;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                return "false";
 
            }
        }

        private string deletemapwindow(JObject jsoncommand)
        {
            try
            {
                string pName = jsoncommand["WINID"].ToString();
                mapwindows mapwindow = LayoutRoot.FindName(pName) as mapwindows;
                if (mapwindow != null)
                {
                    LayoutRoot.Children.Remove(mapwindow);
                    this.UnregisterName(pName);   //add by lzd
                    return pName;
                }else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                return "false";

            }
        }
        #endregion


        private void mainmap_MouseClick(object sender, Map.MouseEventArgs e)
        {
            //MessageBox.Show(e.MapPoint.X.ToString());
        }

        //在地图范围变化后，自动向GQY视频控制器重新定位视频位置  add by lzd
        private void mainmap_ExtentChanged(object sender, ExtentEventArgs e)
        {
            if (GlobalLayers._gqyvideocontrol._videocollection.Count > 0)
            {
                if (GlobalLayers._gqyvideocontrol.linkgqy() != null)
                {
                    foreach (var de in GlobalLayers._gqyvideocontrol._videocollection)
                    {

                        string commandstr = "OpenWin,";
                        MapPoint DM = de.Value as MapPoint;
                        DM.SpatialReference = new SpatialReference(4326);
                        Point winpoint = mainmap.MapToScreen(DM);
                        commandstr += de.Key.ToString() + ",";
                        commandstr += (winpoint.X + 60).ToString() + "," + (winpoint.Y + 60).ToString() + ",";
                        commandstr += "800,600" + "#";
                        //     MessageBox.Show(commandstr);

                        GlobalLayers._gqyvideocontrol.sendcommandTCP(commandstr);
                    }
                    GlobalLayers._gqyvideocontrol.closegqy();
                }
            }
            if (GlobalLayers._gqyvideocontrol._Dynamic_videocollection.Count > 0)
            {
                if (GlobalLayers._gqyvideocontrol.linkgqy() != null)
                {
                    foreach (var dd in GlobalLayers._gqyvideocontrol._Dynamic_videocollection)
                    {
                        string commandstr = "OpenWin,";
                        PointAndDTime PAT = dd.Value as PointAndDTime;
                        MapPoint DDM = PAT.videodot;
                        DDM.SpatialReference = new SpatialReference(4326);
                        Point winpoint = mainmap.MapToScreen(DDM);
                        commandstr += dd.Key.ToString() + ",";
                        commandstr += (winpoint.X + 60).ToString() + "," + (winpoint.Y + 60).ToString() + ",";
                        commandstr += "800,600" + "#";
                        //     MessageBox.Show(commandstr);
                        GlobalLayers._gqyvideocontrol.sendcommandTCP(commandstr);
                    }
                    GlobalLayers._gqyvideocontrol.closegqy();
                }
            }
        }


        private void btnSL_Click(object sender, RoutedEventArgs e)
        {            
            MyTile p = new MyTile();
            p.Url = SL;
            p.ID = "SL";
            p.Visible = true;
            p.EnableOffline = true;
            p.SaveOfflineTiles = true;
            p.LoadOfflineTileFirst = true;
            mainmap.Layers.Clear();
            mainmap.Layers.Add(p);
            mainmap.Extent = new ESRI.ArcGIS.Client.Geometry.Envelope(121.1, 28.5, 121.38, 28.65);
        }

        private void btnYX_Click(object sender, RoutedEventArgs e)
        {
            MyTile p = new MyTile();
            p.Url = YX;
            p.ID = "YX";
            p.Visible = true;
            p.EnableOffline = true;
            p.SaveOfflineTiles = true;
            p.LoadOfflineTileFirst = true;
            mainmap.Layers.Clear();
            mainmap.Layers.Add(p);
            mainmap.Extent = new ESRI.ArcGIS.Client.Geometry.Envelope(121.1, 28.5, 121.38, 28.65);
        }

        private void btnSY_Click(object sender, RoutedEventArgs e)
        {
            MyTile p = new MyTile();
            p.Url = SY;
            p.ID = "SY";
            p.Visible = true;
            p.EnableOffline = true;
            p.SaveOfflineTiles = true;
            p.LoadOfflineTileFirst = true;
            mainmap.Layers.Clear();
            mainmap.Layers.Add(p);
            mainmap.Extent = new ESRI.ArcGIS.Client.Geometry.Envelope(121.1, 28.5, 121.38, 28.65);
        }

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

    }
}
