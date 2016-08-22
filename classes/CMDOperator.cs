using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Local;
using System.Windows.Media.Animation;
using System.Runtime.InteropServices;
using Apache.NMS.ActiveMQ;
using Apache.NMS;

using zsdpmap.parts;

namespace zsdpmap.classes
{

    class CMDOperator
    {
        string commandstr = "";
        public static JObject newJson00 = null;
        public static JObject newJson01 = null;

        System.Collections.ArrayList alPoints = new System.Collections.ArrayList();
        //IList<Graphic> gp = null;
        //GeometryService _geometryTask;
        //Thread threadPointsRun = null;
        GeomServControl gsc = null;
        public static int flagSPNum = 0;
        public static bool isOpened = false;//LPY 2015-1-30 22:51:28 添加 记录视频集合底板是否已打开，打开状态就不关闭
        public static bool isOpenedSPLD = false;//LPY 2015-7-28 22:25:41 添加视频联动版状态标记
        public static bool flagForMFBZD = true;//LPY 2015-8-12 11:52:40 添加 为民防避灾点加载作判断

        public static VideoToWall.B20ControlData[] _b20controldata = new VideoToWall.B20ControlData[6];
        public static VideoToWall.B20ControlData[] _b20controldataSPLD = new VideoToWall.B20ControlData[6];//给视频联动用
        public static VideoToWall.B20ControlData _b20controldataZoomUp=new VideoToWall.B20ControlData();//
        private static int x1 = Convert.ToInt32(ConfigurationManager.AppSettings["x1"]);
        private static int x2 = Convert.ToInt32(ConfigurationManager.AppSettings["x2"]);
        private static int y1 = Convert.ToInt32(ConfigurationManager.AppSettings["y1"]);
        private static int y2 = Convert.ToInt32(ConfigurationManager.AppSettings["y2"]);
        private static int y3 = Convert.ToInt32(ConfigurationManager.AppSettings["y3"]);
        private static int w = Convert.ToInt32(ConfigurationManager.AppSettings["w"]);
        private static int h = Convert.ToInt32(ConfigurationManager.AppSettings["h"]);

        private static int SPLDx1 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDx1"]);
        private static int SPLDx2 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDx2"]);
        private static int SPLDx3 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDx3"]);
        private static int SPLDy1 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDy1"]);
        private static int SPLDy2 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDy2"]);
        private static int SPLDy3 = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDy3"]);
        private static int SPLDw = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDw"]);
        private static int SPLDh = Convert.ToInt32(ConfigurationManager.AppSettings["SPLDh"]);

        private static int videoZoomUpX = Convert.ToInt32(ConfigurationManager.AppSettings["videoZoomUpX"].ToString());
        private static int videoZoomUpY = Convert.ToInt32(ConfigurationManager.AppSettings["videoZoomUpY"].ToString());
        private static int videoZoomUpHeight = Convert.ToInt32(ConfigurationManager.AppSettings["videoZoomUpHeight"].ToString());
        private static int videoZoomUpWidth = Convert.ToInt32(ConfigurationManager.AppSettings["videoZoomUpWidth"].ToString());

        public static JObject xxJsonInfo = null;
        public static JObject jsonForYAHtmlPad = null;//LPY 2015-7-9 14:48:48 添加 用于HTML预案显示
        public static JObject jsonForYATextPad = null;//LPY 2015-7-9 17:07:13 添加 用于文本显示

        public OneVideoInfo oneVideoInfo = null;

        private static IConnectionFactory factory;
        private static IConnection connection;
        private static ISession session = null;
        private static IMessageProducer prod = null;
        private static ITextMessage message = null;
        private static string telnum = string.Empty;
     

        Window MW;

        public CMDOperator(Window _mw)
        {

            MW = _mw;
            InidtProducer();
        }

        private void InidtProducer()
        {
            try
            {
                factory = new ConnectionFactory(System.Configuration.ConfigurationManager.AppSettings["MQ"]);
                connection = factory.CreateConnection();
                session = connection.CreateSession();
                prod = session.CreateProducer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic("TelBox"));
                message = prod.CreateTextMessage();
            }
            catch (Exception)
            {
                
            }
        }
        
        #region 处理收到的指令
        public void operatecommand(JObject rejson)
        {
            if (rejson["CMD"].ToString() == "00105")    // GEO类指令因为响应时间长，需要单独起线程处理
            {
  

                Geometry gm = GeometryFunc.ParsefromJson(rejson["GRAPHIC"] as JObject);

                if (rejson["TYPE"].ToString() == "esriGeometryPolyline") // need buffer
                {
                    int Distance;
                    if (rejson.Property("DISTANCE") != null)
                        Distance = int.Parse(rejson["DISTANCE"].ToString());
                    else
                        Distance = 30;

                    // 绘制线条
          //          GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
          //           new Action(
          //           delegate
                     {
                         Graphic LineGraphic = new Graphic();
                         LineGraphic.Geometry = gm;
                         LineGraphic.Symbol = App.Current.Resources["SLSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                         LineGraphic.SetZIndex(1);
                         GraphicsLayer DrawLayer = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
                         DrawLayer.Graphics.Add(LineGraphic); // 显示Buffer结果
                         DrawLayer.Refresh();
                     }
          //           )
          //           );
                    QueryWithBuffer QWB = new QueryWithBuffer("BufferGraphicsLayer",  Distance);
                    QWB.SetBuffGeom(gm);
                    QWB.ProcessBuffer();

                    //
               //     Thread DoBuffer = new Thread(QWB.ProcessBuffer);
               //     DoBuffer.Start();
                }
                else
                {
                    // 显示需要查询的多边形
       //             GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
       //             new Action(
       //             delegate
       //             {
              Graphic bufferGraphic = new Graphic();
              bufferGraphic.Geometry = gm;
              bufferGraphic.Symbol = App.Current.Resources["BufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
              bufferGraphic.SetZIndex(1);
              GraphicsLayer DrawLayer = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
              DrawLayer.Graphics.Add(bufferGraphic); // 显示查询区域

         //           }
         //           )
         //           );
                    //QueryLayer QL = new QueryLayer(gm, "CJ_SP_PT", "ResultGraphicsLayer");
                    //QL.Do();
                 //   Thread DoProcGeom = new Thread(QL.Do);
                 //   DoProcGeom.Start();
                }
                return;
            }

            GlobalLayers._MainMap.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
            switch (rejson["CMD"].ToString())
            {
                case "00000":
                    // WITCHMAP
                    string map = rejson["WITCHMAP"].ToString();
                    if (map == "slmap") // 矢量
                    {
                        GlobalLayers._MainMap.Layers["SL"].Visible = true;
                        GlobalLayers._MainMap.Layers["YX"].Visible = false;
                    }
                    else
                        if(map =="symap")// 切换影像底图
                    {
                        GlobalLayers._MainMap.Layers["SL"].Visible = false;
                        GlobalLayers._MainMap.Layers["YX"].Visible = true;
                    }
                    break;
                case "10005":   // 车辆离在线显示开关
                    string ss = rejson["VALUE"].ToString();
                    bool s = true;
                    if (ss == "true") // 只显示1线
                        s = true;
                    else
                        s = false;
                    Util.SetSwitchOfflineLayar(s);
                    break;
                case "10004":   // 车辆过滤表指令
                    try
                    {
                        CarFilter.ModifyFilterLock = true; // 在清理图层时，锁定，让接受线程不绘制
                        JArray _tmpfilter = rejson["VALUE"] as JArray;
                        CarFilter.ResetFilter(_tmpfilter);  // 车辆过滤器重置
                        Util.ClearLayer(GlobalLayers._policepointglr); // 清除当前的车辆图层
                        Util.ClearLayer(GlobalLayers._offlineglr);     // 清除当前的离线车辆图层
                        CarFilter.ModifyFilterLock = false;
                    }
                    catch (Exception) { }
                    break;
                case "10003":   // 开关
                    try
                    {
                        GlobalLayers._MainMap.Dispatcher.Invoke(
                            new Action(
                                delegate
                                {
                                    PUBLIC_SWITCH.DoSwitch(rejson["ITEM"].ToString(), rejson["SWITCH"].ToString());
                                }));
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "10002"://LPY 2014-12-15 10:22:21 添加 热力图/聚合图 查询条件
                                                                                                //if (GlobalLayers._HeatMapLayercontrol.AddHeatMapLayer(rejson, GlobalLayers._MainMap) != "false")
                                                                                                //{
                                                                                                //}
                    //LPY 2015-8-11 15:17:11 修改 增加聚合图指令处理Type
                    try
                    {
                        ClusterLayers.DeleteClusterLayerByLayerName();//先关闭可能存在的图层，下句代码一样
                        if (GlobalLayers._HeatMapLayercontrol.DeleteHeatMapLayer( GlobalLayers._MainMap) != "false")
                        {
                        }
                        //if (rejson["type"]==null||rejson["type"].ToString()=="")
                        //{
                        //    break;
                        //}
                        switch (rejson["type"].ToString())
                        {

                            case "HeatMap"://热力图
                                if (GlobalLayers._HeatMapLayercontrol.AddHeartMapLayerWithCondition(rejson, GlobalLayers._MainMap) != "false")
                                {
                                }
                                break;
                            case "clusterer"://聚合图
                                ClusterLayers. DeleteClusterLayerByLayerName();
                                if (ClusterLayers.AddClusterLayerByCondition(rejson, GlobalLayers._MainMap))
                                {
                                }
                                break;
                            default:
                                break;
                        }
                        //if (rejson["type"].ToString() == "HeatMap")
                        //{
                            
                        //}

                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    break;

                case "10001":   // 全关视频指令 //LPY 2015-7-29 10:08:41 注释 不知道gqy何用
                    //if (GlobalLayers._gqyvideocontrol.linkgqy() != null)
                    //{
                        //GlobalLayers._gqyvideocontrol._Dynamic_videocollection.Clear();
                        //GlobalLayers._gqyvideocontrol._videocollection.Clear();
                        //GlobalLayers._gqyvideocontrol.CloseAll();
                    //}
                    break;
                case "20001":   // 模拟车辆跟踪启动
                    GlobalLayers.DynamicGraphicLayer.ClearGraphics();
                    GlobalLayers.DynamicResultGraphicLayer.ClearGraphics();
                    GlobalLayers.DynamicGraphicLayer.Visible = true;        // 用Visiable 是为了解决关闭模拟后，因线程同步原因，清除图层时处理线程再继续往图层放信息，不能清除干净
                    PublicVARS.MainGPS = "00000000"; //  null;
                    PublicVARS.MainVID = null;
                    PublicVARS.MainX = 121.2256;
                    PublicVARS.MainY = 28.5543;

                    PublicVARS.TESTMODE = true;

                    break;
                case "20002":   // 模拟关闭
                    PublicVARS.TESTMODE = false;
                    PublicVARS.MainGPS = null;
                    PublicVARS.MainVID = null;
                    PublicVARS.MainX = 0;
                    PublicVARS.MainY = 0;
                    GlobalLayers._gqyvideocontrol.CloseVID(PublicVARS.MainVID);
                    GlobalLayers.DynamicGraphicLayer.ClearGraphics();
                    GlobalLayers.DynamicResultGraphicLayer.ClearGraphics();
                    GlobalLayers.DynamicGraphicLayer.Visible = false;
                    GlobalLayers.DynamicResultGraphicLayer.Visible = false;
                    GlobalLayers._GraphicLogCollention.Clear();
                    GlobalLayers._gqyvideocontrol.CloseAllFlush();

  
                    break;
                case "80000": // 地图窗口位置模拟
                    break;
                case "00106":                   // 清除手绘图形
                    try
                    {
                        GlobalLayers.BufferGraphicLayer.ClearGraphics();
                        GlobalLayers.ResultGraphicLayer.ClearGraphics();
                        GlobalLayers._GraphicLogCollention.Clear();     // 怎么处理和车辆查询的冲突
                        GlobalLayers._gqyvideocontrol.CloseAllVID();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                    break;
                case "00107":   // 打开INFOWINDOW
                    try
                    {
                        //LogHelper.WriteLog(typeof(MainWindow), rejson.ToString());
                        GlobalLayers.FocusLayer.ClearGraphics();
                        GlobalLayers.InfoWin.IsOpen = false;
                        Geometry INFO_G = null;
                        string TYPE = null;
                        if (rejson.Property("LON") == null)
                            break;
                        Geometry G = new MapPoint(double.Parse(rejson["LON"].ToString()), double.Parse(rejson["LAT"].ToString())); // 显示infowindow 的位置
                        GlobalLayers.InfoWin.Anchor = G as MapPoint; //  e.MapPoint;

                        

                        if (rejson.Property("TYPE") != null)
                        {
                            TYPE = rejson["TYPE"].ToString();
                        }
                        if (rejson.Property("GRAPHIC") != null)
                        {
                            INFO_G = GeometryFunc.ParsefromJson(rejson["GRAPHIC"] as JObject);
                        }
                        if (TYPE != null && INFO_G != null)
                        {
                        }
                        switch (TYPE)
                        {
                            case "esriGeometryPoint":   // 点不绘制

                                break;
                            case "esriGeometryPolygon":
                            case "esriGeometryPolyline":
                                Graphic FocusG = new Graphic();
                                FocusG.Geometry = INFO_G;
                                FocusG.Symbol = App.Current.Resources["BufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                                
                                GlobalLayers.FocusLayer.Graphics.Add(FocusG);
                                break;

                        }
                        if (rejson.Property("TCMC") != null)
                        {
                            string tcmc = "INFO_" + rejson["TCMC"].ToString();
                            string InfoID = System.Configuration.ConfigurationManager.AppSettings[tcmc].ToString();  // 获取图层名称


                            //  G1.Attributes["MC"] = (rejson["GraphicOBJ"] as JObject)["MC"].ToString();
                            //Since a ContentTemplate is defined, Content will define the DataContext for the ContentTemplate
                            GlobalLayers.InfoWin.ContentTemplate = GlobalLayers.LayoutRoot.Resources[InfoID] as DataTemplate;

                            
                        }
                        if (rejson.Property("AttrStr") != null)
                        {
                            GlobalLayers.InfoWin.Content = rejson["AttrStr"] as JObject;
                        }
                        GlobalLayers.InfoWin.IsOpen = true;


                        if (rejson["TCMC"].ToString() == "BZD_PG")//表示点击了避灾点
                        {
                            GlobalLayers.InfoWin.IsOpen = false;

                            string tcmc = "INFO_" + rejson["TCMC"].ToString();
                            string InfoID = System.Configuration.ConfigurationManager.AppSettings[tcmc].ToString();

                            GlobalLayers.InfoWinMF.ContentTemplate = GlobalLayers.LayoutRoot.Resources[InfoID] as DataTemplate;
                            if (rejson.Property("AttrStr") != null)
                            {
                                GlobalLayers.InfoWinMF.Content = rejson["AttrStr"] as JObject;
                            }
                            GlobalLayers.InfoWinMF.Anchor = G as MapPoint;
                            GlobalLayers.InfoWinMF.IsOpen = true;
                        }

                        ////LPY 2015-7-31 11:00:05 添加 点击民防应急疏散线路时，能够显示动态线路。。。。。
                        if (rejson["TCMC"].ToString() == "LINE_PL")//表示点击了线路
                        {
                        //    ClearMFLayers();
                            int num = -1;
                            string strLineName = rejson["AttrStr"]["DLMC"].ToString();
                            if (strLineName != null)
                            {
                                if (strLineName == "院桥街至岙里街村疏散路线")
                                    num = 0;
                                else if (strLineName == "前庄至占堂村疏散路线")
                                    num = 2;
                                else if (strLineName == "下抱至繁荣村疏散路线")
                                    num = 1;
                            }
                            if (num == -1)
                            {
                                break;
                            }
                        //    FeatureLayer oneLine = GlobalLayers._MainMap.Layers["LINE_PL"] as FeatureLayer;
                        //    Graphic _g = new Graphic();
                        //    _g = oneLine.Graphics[num] as Graphic;
                        //    //动态线条，显示用，关闭视频巡逻时要关闭
                        //    GraphicsLayer graphicsLayerMove = new GraphicsLayer();
                        //    graphicsLayerMove.ID = "graphicsLayerMove";
                        //    graphicsLayerMove.Visible = true;
                        //    GlobalLayers._MainMap.Layers.Add(graphicsLayerMove);
                        //    Graphic moveLine = new Graphic()
                        //    {
                        //        Symbol = App.Current.Resources["MFYALine1"] as Symbol,//动态线条
                        //        Geometry = ReverseGeometry(_g.Geometry)//_g.Geometry //ReverseGeometry(_g.Geometry)
                        //    };
                        //    graphicsLayerMove.Graphics.Add(moveLine);

                            ShowMFBZDByNum(num); 
                        }

                        if (rejson["TCMC"].ToString() == "MF_PG")
                        {
                            switch (rejson["AttrStr"]["SQMC"].ToString())
                            {
                                case "岙里街":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.aolijie);
                                    break;
                                case "院桥街":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.yuanqiaojie);
                                    break;
                                case "下抱":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.xiabao);
                                    break;
                                case "前庄":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.qianzhuang);
                                    break;
                                case "繁荣":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.fanrong);
                                    break;
                                case "占堂":
                                    GlobalLayers.InfoWin.Content = JObject.Parse(GlobalLayers.zhantang);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "00108":   //  INFOwindow 关闭
                    GlobalLayers.FocusLayer.ClearGraphics();
                    GlobalLayers.InfoWin.IsOpen = false;
                    GlobalLayers.InfoWinMF.IsOpen = false;
                    break;


                case "00001":
                    //this.mainmap.Dispatcher.Invoke(
                    //    new Action(
                    //         delegate
                    //         {
                    GlobalLayers._mapcontrol.ZoomPoint(rejson);
                    //     }
                    //            )
                    //);  
                    break;

                case "00002": // mouse move
                    
                    break;

                case "00021":   // 打开窗口
                    if (createmapwindow(rejson) != "false")
                    {
                    }
                    else
                    {
                    }
                    break;

                case "00022":
                    if (deletemapwindow(rejson) != "false")
                    {
                    }
                    else
                    {
                    }
                    break;

                case "00031":

                    //     if (rejson["LAYER"].ToString().Trim().Substring(0, 8) != "HEATMAP_" && rejson["LAYER"].ToString().Trim() != "AFD")

                    if (rejson["LAYER"].ToString() != "AFD")
                    {
                        if (GlobalLayers._featurelayercontrol.AddFeaturelayer(rejson, GlobalLayers._MainMap) != "false")
                        {
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        if (GlobalLayers._HeatMapLayercontrol.AddHeatMapLayer(rejson, GlobalLayers._MainMap) != "false")
                        {
                        }
                    }

                    break;

                case "00032":
                    //      if (rejson["LAYER"].ToString().Trim().Substring(0, 8) == "HEATMAP_" && rejson["LAYER"].ToString().Trim() != "AFD")
                    if (rejson["LAYER"].ToString() != "AFD")
                    {
                        if (GlobalLayers._featurelayercontrol.DeleteFeaturelayer(rejson, GlobalLayers._MainMap) != "false")
                        {
                        }
                    }
                    else
                    {
                        ClusterLayers.DeleteClusterLayerByLayerName();//LPY 2015-8-11 18:49:48 添加此行，删除聚合图层，下面一句删除热力图层。
                        if (GlobalLayers._HeatMapLayercontrol.DeleteHeatMapLayer(rejson, GlobalLayers._MainMap) != "false")
                        {
                        }
                    }
                    break;

                case "00041":
                    break;//LPY 2015-3-13 22:09:32 这里暂时未用到 注释掉
                    commandstr = "OpenWin,";
                    MapPoint OP = new MapPoint((double)rejson["X"], (double)rejson["Y"]);
                    OP.SpatialReference = new SpatialReference(4326);
                    Point winpoint = GlobalLayers._MainMap.MapToScreen(OP);
                    commandstr = commandstr + rejson["VID"].ToString() + ",";
                    commandstr = commandstr + (winpoint.X + 60).ToString() + "," + (winpoint.Y +60).ToString() + ",";
                    commandstr = commandstr + "800,600" + "#";
                    if (GlobalLayers._gqyvideocontrol.linkgqy() != null)  // 是否把错误状态都传给控制台
                    {
                        GlobalLayers._gqyvideocontrol.sendcommandTCP(commandstr);
                        GlobalLayers._gqyvideocontrol.closegqy();
/*
                        if (GlobalLayers._gqyvideocontrol._videocollection.ContainsKey(rejson["VID"].ToString()))     // 更新视频ID及其位置，用于地图缩放平移后向GQY刷新位置
                            GlobalLayers._gqyvideocontrol._videocollection.Remove(rejson["VID"].ToString());          // add by john 因为现在open已经改为可重复打开
                        GlobalLayers._gqyvideocontrol._videocollection.Add(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"])); //add by lzd
  */

                        GlobalLayers._gqyvideocontrol._videocollection.AddOrUpdate(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]), (key,value) => new MapPoint((double)rejson["X"], (double)rejson["Y"]));
                    }
                    break;

                case "00042":   // 关闭GQY 视频
                        commandstr = "CloseWin," + rejson["VID"].ToString() + "#";
                        if (GlobalLayers._gqyvideocontrol.linkgqy() != null)
                        {
                            GlobalLayers._gqyvideocontrol.sendcommandTCP(commandstr);
                            GlobalLayers._gqyvideocontrol.closegqy();
                            /*
                            if (GlobalLayers._gqyvideocontrol._videocollection.ContainsKey(rejson["VID"].ToString()))
                                GlobalLayers._gqyvideocontrol._videocollection.Remove(rejson["VID"].ToString());        //add by lzd
                             */
                            Object OutObj;
                            GlobalLayers._gqyvideocontrol._videocollection.TryRemove(rejson["VID"].ToString(),out OutObj);
                        }
             //       }
                    break;

                case "00043":       // 目前没有用到
                    commandstr = "MoveWin,";
                    Point winpointt = GlobalLayers._MainMap.MapToScreen(new MapPoint((double)rejson["X"], (double)rejson["Y"]));
                    commandstr = commandstr + rejson["VID"].ToString() + ",";
                    commandstr = commandstr + (winpointt.X+60).ToString() + "," + (winpointt.Y+60).ToString() + ",";
                    commandstr = commandstr + "800,600" + "#";

                    if (GlobalLayers._gqyvideocontrol.linkgqy() != null)
                    {
                        GlobalLayers._gqyvideocontrol.sendcommandTCP(commandstr);
                        GlobalLayers._gqyvideocontrol.closegqy();

                        GlobalLayers._gqyvideocontrol._videocollection.AddOrUpdate(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]), (key, value) => new MapPoint((double)rejson["X"], (double)rejson["Y"]));
                        /*
                        GlobalLayers._gqyvideocontrol._videocollection.Remove(rejson["VID"].ToString());      //add by lzd
                        GlobalLayers._gqyvideocontrol._videocollection.Add(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]));   
                         */
                    }
                //add by lzd   
                    break;

                case "00080":

                    if (!GlobalLayers._policepointcontrol.addpolicecarpoint(rejson))
                    {
                        //add by lzd
                        if (rejson["VIDEOONOFF"].ToString() == "1")
                        {
                            GlobalLayers._gqyvideocontrol._videocollection.AddOrUpdate(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]), (key, value) => new MapPoint((double)rejson["X"], (double)rejson["Y"]));
/*
                            if (GlobalLayers._gqyvideocontrol._videocollection[rejson["Title"].ToString().Trim()] == null)
                            {
                                GlobalLayers._gqyvideocontrol._videocollection.Add(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]));
                            }
                            else
                            {
                                GlobalLayers._gqyvideocontrol._videocollection.Remove(rejson["VID"].ToString());
                                GlobalLayers._gqyvideocontrol._videocollection.Add(rejson["VID"].ToString(), new MapPoint((double)rejson["X"], (double)rejson["Y"]));
                            }
 */
                        }
                        else
                        {
                            Object OutObj;
                            GlobalLayers._gqyvideocontrol._videocollection.TryRemove(rejson["VID"].ToString(), out OutObj);
 
                            /*
                            GlobalLayers._gqyvideocontrol._videocollection.Remove(rejson["VID"].ToString());
                             */

                        }
                    }
                    if (rejson.Property("HH") != null)  // 简单处理， 有GPS 呼号的点才处理刷新
                    {
                        if (PublicVARS.MainGPS == rejson["HH"].ToString())
                        {
                            PublicVARS.MainX = (double)rejson["X"];
                            PublicVARS.MainY = (double)rejson["Y"];
                            PublicVARS.Speed = (double)rejson["SPEED"];
                            PublicVARS.Direct = (double)rejson["DIRECT"];
                            //      QueryWithBuffer QWB = new QueryWithBuffer(LayoutRoot, mainmap, GG);
                            //      QWB.DoQueryVideo((double)rejson["X"], (double)rejson["Y"]);
                        }
                    }
                    break;

                case "00090":
                    if (openwindow(rejson) != "false")
                    {
                    }
                    else
                    {
                    }
                    break;

                case "00091":
                    if (closewindow(rejson) != "false")
                    {
                    }
                    else
                    {
                    }
                    break;
                case "00103":   // 指定主控车
                    try
                    {
                        if (rejson["STATE"].ToString() == "ON")
                        {
                            GlobalLayers.DynamicGraphicLayer.ClearGraphics();
                            GlobalLayers.DynamicResultGraphicLayer.ClearGraphics();
                            GlobalLayers._GraphicLogCollention.Clear();
                            GlobalLayers._gqyvideocontrol.CloseAllFlush();
                            PublicVARS.MainX = 0.0;
                            PublicVARS.MainY = 0.0;
                            String GPSID = rejson["GPSID"].ToString();
                            String VID = rejson["VID"].ToString();
                            PublicVARS.MainGPS = GPSID;
                            PublicVARS.MainVID = VID;

  
                            GlobalLayers.DynamicGraphicLayer.Visible = true;        // 用Visiable 是为了解决关闭模拟后，因线程同步原因，清除图层时处理线程再继续往图层放信息，不能清除干净
                            GlobalLayers.DynamicResultGraphicLayer.Visible = true;
                        }
                    }
                    catch (Exception)
                    { }
                    break;
                case "00104":   // 取消指定主控车
                    try
                    {
                        if (rejson["STATE"].ToString() == "OFF")
                        {
                            GlobalLayers._gqyvideocontrol.CloseVID(PublicVARS.MainVID);
                            if (PublicVARS.MainVID != null)
                            {
                                Object OutObj;
                                GlobalLayers._gqyvideocontrol._videocollection.TryRemove(PublicVARS.MainVID, out OutObj);
                            }
                            /*
                            if (GlobalLayers._gqyvideocontrol._videocollection.ContainsKey(GlobalLayers.MainVID))    // 更新视频ID及其位置，用于地图缩放平移后向GQY刷新位置
                                GlobalLayers._gqyvideocontrol._videocollection.Remove(GlobalLayers.MainVID);          // add by john 因为现在open已经改为可重复打开
                            */
                            PublicVARS.MainGPS = null;
                            PublicVARS.MainVID = null;
                            PublicVARS.MainX = 0.0;
                            PublicVARS.MainY = 0.0;
             
                            GlobalLayers.DynamicGraphicLayer.ClearGraphics();
                            GlobalLayers.DynamicResultGraphicLayer.ClearGraphics();
                            GlobalLayers._GraphicLogCollention.Clear();
                            GlobalLayers._gqyvideocontrol.CloseAllFlush();
 
                            GlobalLayers.DynamicGraphicLayer.Visible = false;        // 用Visiable 是为了解决关闭模拟后，因线程同步原因，清除图层时处理线程再继续往图层放信息，不能清除干净
                            GlobalLayers.DynamicResultGraphicLayer.Visible = false;

                       //     GlobalLayers._gqyvideocontrol.CloseCARVID();
                 
                        }
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "00101"://LPY 2014-12-29 13:31:31 添加 接收轨迹点并绘制出来
                    try
                    {
                        //Geometry gm = GeometryFunc.ParsefromJson(rejson["FILTER"] as JObject);
                        string CarLine = GetGuiJi(rejson["FILTER"].ToString());
                        Geometry gm = Geometry.FromJson(CarLine);
                        Graphic bufferGraphic = new Graphic();
                        bufferGraphic.Geometry = gm;
                        bufferGraphic.Symbol = App.Current.Resources["SLSymbolGJ"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                        bufferGraphic.SetZIndex(1);
                        GraphicsLayer DrawLayer = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
                        DrawLayer.Graphics.Clear();
                        GlobalLayers.FocusLayer.ClearGraphics();//这两句在显示轨迹的时候关掉InfoWindow 避免轨迹被挡到
                        GlobalLayers.InfoWin.IsOpen = false;
                        DrawLayer.Graphics.Add(bufferGraphic); // 显示查询区域
                    }
                    catch (Exception)
                    {
                        
                    }                    
                    break;
                case "00102":
                    GraphicsLayer DrawLayerNeedClose = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
                    DrawLayerNeedClose.Graphics.Clear();
                    break;

                case "00033"://LPY 2015-1-13 17:47:07 添加 民防 疏散图层
                    try
                    {
                        //string MF_PG_URL = ConfigurationManager.AppSettings["MF_PG"];
                        //string LINE_PL_URL = ConfigurationManager.AppSettings["LINE_PL"];                        
                        //double opacity = Convert.ToDouble(ConfigurationManager.AppSettings["opacity"]);

                        //FeatureLayer mfpglyr = new FeatureLayer();
                        //mfpglyr.Url = MF_PG_URL;
                        //mfpglyr.ID = "MF_PG";
                        //mfpglyr.Opacity = opacity;
                        //mfpglyr.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(mfpglyr);

                        //FeatureLayer linepllyr = new FeatureLayer();
                        //linepllyr.Url = LINE_PL_URL;
                        //linepllyr.ID = "LINE_PL";
                        //linepllyr.Opacity = opacity;
                        //linepllyr.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(linepllyr);
                        
                        //string MF_BZD_URL = ConfigurationManager.AppSettings["MF_BZD"];
                        //FeatureLayer mfbzdlyr = new FeatureLayer();
                        //mfbzdlyr.Url = MF_BZD_URL;
                        //mfbzdlyr.ID = "MF_BZD";
                        //mfbzdlyr.Opacity = opacity;
                        //mfbzdlyr.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(mfbzdlyr);

                        mapcontrol mcHelper = new mapcontrol();
                        FeatureLayer mfpglyr_needshow = mcHelper.getFlyr("MF_PG", GlobalLayers._MainMap);
                        if (mfpglyr_needshow != null)
                        {
                            mfpglyr_needshow.Visible = true;
                        }
                        FeatureLayer linepllyr_needshow = mcHelper.getFlyr("LINE_PL", GlobalLayers._MainMap);
                        if (linepllyr_needshow != null)
                        {
                            linepllyr_needshow.Visible = true;
                        }

                        GraphicsLayer glShelter = GlobalLayers._MainMap.Layers["glShelter"] as GraphicsLayer;
                        glShelter.Visible = true;

                        //LPY 2015-8-11 19:47:42 添加 新增线路巡逻信息窗显示
                        GlobalLayers._yaroadinfopad.Height = 1;
                        GlobalLayers._yaroadinfopad.Width = 1;

                        //LPY 2015-8-11 23:07:45 添加 把6个避灾点做成3个图层，控制其显隐以实现点击线路，避灾点高亮显示
                        GraphicsLayer glLine1 = GlobalLayers._MainMap.Layers["glMFBZDLine1"] as GraphicsLayer; glLine1.Graphics.Clear(); glLine1.Visible = false;
                        GraphicsLayer glLine2 = GlobalLayers._MainMap.Layers["glMFBZDLine2"] as GraphicsLayer; glLine2.Graphics.Clear(); glLine2.Visible = false;
                        GraphicsLayer glLine3 = GlobalLayers._MainMap.Layers["glMFBZDLine3"] as GraphicsLayer; glLine3.Graphics.Clear(); glLine3.Visible = false;
                        FeatureLayer flSixBZD = GlobalLayers._MainMap.Layers["MF_PG"] as FeatureLayer;
                        Symbol sTemp = App.Current.Resources["BufferSymbol"] as Symbol;
                        Graphic gTemp = null;
                        
                        
                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[0] as Graphic).Geometry, Symbol = sTemp }; glLine1.Graphics.Add(gTemp);                        
                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[4] as Graphic).Geometry, Symbol = sTemp }; glLine1.Graphics.Add(gTemp);

                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[2] as Graphic).Geometry, Symbol = sTemp }; glLine2.Graphics.Add(gTemp);
                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[3] as Graphic).Geometry, Symbol = sTemp }; glLine2.Graphics.Add(gTemp);

                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[5] as Graphic).Geometry, Symbol = sTemp }; glLine3.Graphics.Add(gTemp);
                        gTemp = new Graphic() { Geometry = (flSixBZD.Graphics[1] as Graphic).Geometry, Symbol = sTemp }; glLine3.Graphics.Add(gTemp);

                        //Graphic gTxtTemp = null;
                        //TextSymbol sTxtTemp = App.Current.Resources["TxtSymbol"] as TextSymbol;

                        //sTxtTemp.Text = "院桥街";
                        //gTxtTemp = new Graphic() { Geometry = (flSixBZD.Graphics[0] as Graphic).Geometry, Symbol = sTxtTemp, }; glLine1.Graphics.Add(gTxtTemp);
                        //_gg = flSixBZD.Graphics[4] as Graphic;
                        //GraphicsLayer glt = GlobalLayers._MainMap.Layers["glMFBZD"] as GraphicsLayer;

                        //Graphic hh = new Graphic() { Geometry=_gg.Geometry,Symbol = App.Current.Resources["BufferSymbol"] as Symbol                            
                        //};

                        //Graphic _gg1 = six.Graphics[5] as Graphic;
                        //Graphic hh1 = new Graphic() { Geometry = _gg1.Geometry, Symbol = App.Current.Resources["AreaSymbol"] as Symbol };
                        //GraphicsLayer ggg = new GraphicsLayer();
                        //ggg.ID = "gggg";
                        //ggg.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(ggg);
                        //ggg.Graphics.Add(hh);
                        //ggg.Graphics.Add(hh1);
                        //glt.Graphics.Add(hh);
                        //glt.Graphics.Add(hh1);

                        ShowMFLines();//LPY 2016-5-3 添加 一次性显示三条动态的疏散线路

                    }
                    catch (Exception)
                    {
                    }
                    //打开民放应急疏散图层的时候，显示水库信息，隐藏案件信息。
                    //try
                    //{
                    //    GlobalLayers._pinfowindow.Width = 1;
                    //    GlobalLayers._pinfowindow.Height = 1;
                    //    GlobalLayers._skinfowindow.Width = 1500;//1500;// 850;
                    //    GlobalLayers._skinfowindow.Height = 2400;//2160
                    //}
                    //catch (Exception)
                    //{
                        
                    //}
                    break;
                case "00034"://LPY 2015-1-13 18:19:14 添加 民防 关闭疏散图层
                    GlobalLayers._yaroadinfopad.Height = 1;
                    GlobalLayers._yaroadinfopad.Width = 1;
                    ClearMFLayers();
                    GraphicsLayer graphicsLayerMove_needoff = GlobalLayers._MainMap.Layers["graphicsLayerMove"] as GraphicsLayer;
                    if (graphicsLayerMove_needoff!=null)
                    {
                        GlobalLayers._MainMap.Layers.Remove(graphicsLayerMove_needoff);
                    }
                    mapcontrol _mapcontrol = new mapcontrol();
                    FeatureLayer mfpglyr_needoff = _mapcontrol.getFlyr("MF_PG", GlobalLayers._MainMap);
                    if (mfpglyr_needoff!=null)
                    {
                        mfpglyr_needoff.Visible = false;
                        //GlobalLayers._MainMap.Layers.Remove(mfpglyr_needoff);
                    }
                    FeatureLayer linepllyr_needoff = _mapcontrol.getFlyr("LINE_PL", GlobalLayers._MainMap);
                    if (linepllyr_needoff != null)
                    {
                        linepllyr_needoff.Visible = false;
                        //GlobalLayers._MainMap.Layers.Remove(linepllyr_needoff);
                    }
                    //FeatureLayer mfbzdlyr_needoff = _mapcontrol.getFlyr("MF_BZD", GlobalLayers._MainMap);
                    //if (mfbzdlyr_needoff != null)
                    //{
                    //    GlobalLayers._MainMap.Layers.Remove(mfbzdlyr_needoff);
                    //}

                    //打开民放应急疏散图层的时候，显示水库信息，隐藏案件信息。
                    //try
                    //{
                        //GlobalLayers._pinfowindow.Width = 1500;//temp
                        //GlobalLayers._pinfowindow.Height = 1200;//Temp
                        //GlobalLayers._skinfowindow.Width = 1;
                        //GlobalLayers._skinfowindow.Height = 1;
                    //}
                    //catch (Exception)
                    //{

                    //}

                    //LPY 2015-8-11 19:47:42 添加 新增线路巡逻信息窗显示-隐藏显示
                    
                    GraphicsLayer glShelter1 = GlobalLayers._MainMap.Layers["glShelter"] as GraphicsLayer;
                    glShelter1.Visible = false;
                    CloseMFBZDShow();
                    ClosePlotsShow();
                    break;

                case "00035"://LPY 2015-1-13 20:07:39 添加 桌面打开视频巡逻按钮                                            
                    try
                    {
                        //if (QueryLayer._currentVideo.DB33 != "")
                        //{
                        //    VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                        //}
                        CloseSPXLVideo();//
                        //ClearMFLayers();
                        int num = -1;
                        if (rejson["Layer"] != null)
                        {
                            if (rejson["Layer"].ToString() == "院桥街至岙里街村疏散路线")
                                num = 0;
                            else if (rejson["Layer"].ToString() == "前庄至占堂村疏散路线")
                                num = 2;
                            else if (rejson["Layer"].ToString() == "下抱至繁荣村疏散路线")
                                num = 1;
                        }
                        if (num==-1)
                        {
                            break;
                        }

                        //添加graphicsLayerLine图层，放置对疏散路线加密了的点
                        GraphicsLayer graphicsLayerLine = new GraphicsLayer();
                        graphicsLayerLine.ID = "graphicsLayerLine";
                        graphicsLayerLine.Visible = true;
                        GlobalLayers._MainMap.Layers.Add(graphicsLayerLine);

                        FeatureLayer oneLine = GlobalLayers._MainMap.Layers["LINE_PL"] as FeatureLayer;
                        Graphic _g = new Graphic();                        
                        _g= oneLine.Graphics[num] as Graphic;

                        

                        ////动态线条，显示用，关闭视频巡逻时要关闭
                        //GraphicsLayer graphicsLayerMove = new GraphicsLayer();
                        //graphicsLayerMove.ID = "graphicsLayerMove";
                        //graphicsLayerMove.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(graphicsLayerMove);
                        //Graphic moveLine = new Graphic()
                        //{
                        //    Symbol = App.Current.Resources["MFYALine1"] as Symbol,//动态线条
                        //    Geometry = ReverseGeometry(_g.Geometry)//_g.Geometry //ReverseGeometry(_g.Geometry)
                        //};
                        //graphicsLayerMove.Graphics.Add(moveLine);
                        
                        GraphicsLayer graphicsLayerTemp = new GraphicsLayer();//临时图层，用来存放单条疏散路线，并准备对该线路的点进行加密，加密完成后废弃
                        graphicsLayerTemp.ID = "graphicsLayerTemp";
                        //graphicsLayerTemp.Visible = true;
                        //GlobalLayers._MainMap.Layers.Add(graphicsLayerTemp);//不添加到地图上，只用来做加密用
                        Graphic gg = new Graphic() {
                            Symbol = App.Current.Resources["DefaultLineSymbol"] as Symbol,//MFYALine1
                            Geometry=_g.Geometry
                        };
                        graphicsLayerTemp.Graphics.Add(gg);
                        DensifyParameters densityParameters = new DensifyParameters()
                        {
                            LengthUnit = LinearUnit.Meter,
                            Geodesic = true,
                            MaxSegmentLength = Convert.ToDouble(ConfigurationManager.AppSettings["MFLenMeter"])//100//GlobalLayers._MainMap.Resolution* 1
                        };

                        GraphicsLayer graphicsLayerLinePointsRun = new GraphicsLayer();//该图层用来存放扩大了的当前点，呈现闪烁状态，实际应用中，利用当前点进行视频点查找并打开视频
                        graphicsLayerLinePointsRun.ID = "graphicsLayerLinePointsRun";
                        graphicsLayerLinePointsRun.Visible = true;
                        GlobalLayers._MainMap.Layers.Add(graphicsLayerLinePointsRun);

                        //对疏散线路的点进行加密，以期更完整的查找到线路沿途所有的视频点
                        gsc = new GeomServControl();
                        gsc._geometryTask.DensifyAsync(graphicsLayerTemp.Graphics.ToList(), densityParameters);
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "00036"://LPY 2015-1-13 20:06:49 添加 桌面关闭视频巡逻  
                    //if (QueryLayer._currentVideo.DB33 != "")
                    //{
                    //    VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                    //}
                    CloseSPXLVideo();
                    ClearMFLayers();//清除民防相关图层
                    break;
                case "00037"://LPY 2015-1-21 21:54:27 添加 弹出视频窗
                    try
                    {
                        //flagSPNum = 0;//初始化视频打开个数计数器
                        //CloseAllSP();//检查有无已打开的视频，并关闭
                        if (!isOpened)//已打开状态下，不关闭
                        {
                            GlobalLayers._spinfowindow.Width = 1700;
                            GlobalLayers._spinfowindow.Height = 2160;
                            ThicknessAnimation taIn = new ThicknessAnimation();
                            taIn.From = new Thickness(1700, 0, 0, 0);
                            taIn.To = new Thickness(0, 0, 0, 0);
                            taIn.Duration = TimeSpan.FromSeconds(1);
                            GlobalLayers._spinfowindow.BeginAnimation(UserControl.BorderThicknessProperty, taIn);
                            isOpened = true;
                        }                        
                    }
                    catch (Exception)
                    {
                        
                    }
                    try
                    {
                        GraphicsLayer graphicsLayerSP = new GraphicsLayer();//该图层用来存放扩大了的当前点，呈现闪烁状态，实际应用中，利用当前点进行视频点查找并打开视频
                        graphicsLayerSP.ID = "graphicsLayerSP";
                        graphicsLayerSP.Visible = true;                        
                        GlobalLayers._MainMap.Layers.Add(graphicsLayerSP);
                    }
                    catch (Exception)
                    {
                        
                    }
                    break;
                case "00038"://LPY 2015-1-21 22:05:38 添加 关闭视频窗
                    try
                    {
                        flagSPNum = 0;//初始化视频打开个数计数器
                        ClearAllVideos();//CloseAllSP();//检查有无已打开的视频，并关闭
                        //ThicknessAnimation taOut = new ThicknessAnimation();
                        //taOut.From = new Thickness(0, 0, 0, 0);
                        //taOut.To = new Thickness(1700, 0, 0, 0);
                        //taOut.Duration = TimeSpan.FromSeconds(1);
                        //GlobalLayers._spinfowindow.BeginAnimation(UserControl.BorderThicknessProperty, taOut);
                        isOpened = false;
                        //GlobalLayers._spinfowindow.Width = 1;
                        //GlobalLayers._spinfowindow.Height = 1;
                        InitSPINFOPAD();
                    }
                    catch (Exception)
                    {
                        
                    }

                    try
                    {
                        //如果存在graphicsLayerSP图层，则删除
                        GraphicsLayer graphicsLayerSP_needoff = GlobalLayers._MainMap.Layers["graphicsLayerSP"] as GraphicsLayer;
                        if (graphicsLayerSP_needoff != null)
                        {
                            GlobalLayers._MainMap.Layers.Remove(graphicsLayerSP_needoff);
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    //GlobalLayers.LayoutRoot.Children.Add(GlobalLayers._spinfowindow);
                    //GlobalLayers._spinfowindow = null;
                    break;
                case "00039"://LPY 2015-1-21 22:05:38 添加 打开一路视频（序号，视频编号，视频名称，X,Y）
                    if (rejson["SPXX"]!=null)
                    {
                        try
                        {
                            CloseAllSPSPLD();
                            flagSPNum++;//标记视频位置已被占用个数，总共6个
                            Graphic graphicSP = new Graphic();                        
                            graphicSP.Geometry = new MapPoint(Convert.ToDouble(rejson["SPXX"]["geometry"]["x"].ToString()), Convert.ToDouble(rejson["SPXX"]["geometry"]["y"].ToString()), new SpatialReference(4326));
                            graphicSP.Symbol = App.Current.Resources["SPNum"] as Symbol;
                            graphicSP.Attributes["ID"] = rejson["ID"].ToString();
                            graphicSP.Attributes["STDID"] = rejson["SPXX"]["attributes"]["STDID"].ToString();
                            graphicSP.SetZIndex(100);
                            GraphicsLayer graphicsLayerSP = GlobalLayers._MainMap.Layers["graphicsLayerSP"] as GraphicsLayer;
                            if (graphicsLayerSP != null)
                            {                                
                                graphicsLayerSP.Graphics.Add(graphicSP);
                                
                            }
                            //根据flagSPNum的值确定当前视频该排放在哪个位置
                            UpdateSPLabel(rejson["SPXX"]["attributes"]["MC"].ToString(), rejson["ID"].ToString(), rejson["SPXX"]["attributes"]["STDID"].ToString());
                        }
                        catch (Exception)
                        {
                            
                        }
                        

                    }
                    break;
                case "00040"://LPY 2015-1-21 22:05:38 添加 关闭一路视频（序号，视频编号，视频名称，X,Y）
                    if (rejson["SPXX"] != null)
                    {
                        try
                        {
                            //CloseAllSPSPLD();
                            flagSPNum--;//mapcontrol.getMapG(HH, GlobalLayers._policepointglr)
                            //string spid = rejson["ID"].ToString();
                            string spid = rejson["SPXX"]["attributes"]["STDID"].ToString();
                            GraphicsLayer graphicsLayerSP = GlobalLayers._MainMap.Layers["graphicsLayerSP"] as GraphicsLayer;
                            if (graphicsLayerSP != null)
                            {
                                Graphic graphicSP_needoff = mapcontrol.getMapGByStdid(spid, graphicsLayerSP);
                                if (graphicSP_needoff != null)
                                {
                                    graphicsLayerSP.Graphics.Remove(graphicSP_needoff);
                                }
                            }
                            //UpdateSPLabelOff(spid, rejson["SPXX"]["attributes"]["STDID"].ToString());
                            UpdateSPLabelOff(spid, rejson["SPXX"]["attributes"]["STDID"].ToString());
                        }
                        catch (Exception)
                        {
                            
                        }                    
                    }
                    break;
                case "00045"://LPY 2015-3-14 10:54:44 添加 大屏显示信息
                    xxJsonInfo = rejson;
                    GlobalLayers._xxinfowindow.Width = 3200;
                    GlobalLayers._xxinfowindow.Height = 2400;
                    ////GlobalLayers._xxinfowindow.Margin=
                    GlobalLayers._xxinfowindow.FillData();
                    break;
                case "00046"://LPY 2015-3-14 12:29:52 添加 关闭大屏上显示信息的窗口
                    GlobalLayers._xxinfowindow.Width = 1;
                    GlobalLayers._xxinfowindow.Height = 1;
                    break;
                case "00047"://LPY 2015-3-25 09:56:23 添加 显示客户端的测距、测面积功能
                    try
                    {
                        GlobalLayers.BufferGraphicLayer.ClearGraphics();
                        Geometry gm47 = GeometryFunc.ParsefromJson(rejson["GRAPHIC"] as JObject);
                        if (rejson["TYPE"].ToString() == "esriGeometryPolyline")
                        {
                            Graphic lineGraphic47 = new Graphic();
                            lineGraphic47.Geometry = gm47;
                            lineGraphic47.Symbol = App.Current.Resources["LengthSymbol"] as Symbol;
                            lineGraphic47.SetZIndex(1);
                            GraphicsLayer drawLayer47 = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
                            drawLayer47.Graphics.Add(lineGraphic47);
                            JArray jar47 = new JArray();
                            if (rejson["GRAPHIC"]["geometry"]["paths"].ToString()!="")
                            {
                                jar47 = JArray.Parse(rejson["GRAPHIC"]["geometry"]["paths"].ToString());
                                jar47 = JArray.Parse(jar47[0].ToString());
                            }
                            Graphic txtGraphic47 = new Graphic();
                            TextSymbol txtSymbol=new TextSymbol();
                            txtSymbol = App.Current.Resources["TxtSymbol"] as TextSymbol;
                            txtSymbol.Text = rejson["result"].ToString();
                            txtGraphic47.Geometry = new MapPoint(Convert.ToDouble(jar47[jar47.Count - 1][0].ToString()), Convert.ToDouble(jar47[jar47.Count - 1][1].ToString()));
                            txtGraphic47.Symbol = txtSymbol;
                            txtGraphic47.SetZIndex(2);
                            drawLayer47.Graphics.Add(txtGraphic47);

                            drawLayer47.Refresh();
                        }
                        else if (rejson["TYPE"].ToString() == "esriGeometryPolygon")
                        {
                            Graphic bufferGraphic47 = new Graphic();                            
                            bufferGraphic47.Geometry = gm47;
                            bufferGraphic47.Symbol = App.Current.Resources["AreaSymbol"] as Symbol;
                            bufferGraphic47.SetZIndex(1);
                            GraphicsLayer drawLayer47 = GlobalLayers._MainMap.Layers["BufferGraphicsLayer"] as GraphicsLayer;
                            drawLayer47.Graphics.Add(bufferGraphic47);
                            JArray jar47 = new JArray();
                            if (rejson["GRAPHIC"]["geometry"]["rings"].ToString() != "")
                            {
                                jar47 = JArray.Parse(rejson["GRAPHIC"]["geometry"]["rings"].ToString());
                                jar47 = JArray.Parse(jar47[0].ToString());
                            }
                            Graphic txtGraphic47 = new Graphic();
                            TextSymbol txtSymbol = App.Current.Resources["TxtSymbol"] as TextSymbol;
                            txtSymbol.Text = rejson["result"].ToString();
                            txtGraphic47.Geometry = new MapPoint(Convert.ToDouble(jar47[jar47.Count - 1][0].ToString()), Convert.ToDouble(jar47[jar47.Count - 1][1].ToString()));
                            txtGraphic47.Symbol = txtSymbol;
                            txtGraphic47.SetZIndex(2);
                            drawLayer47.Graphics.Add(txtGraphic47);
                            drawLayer47.Refresh();
                        }
                    }
                    catch (Exception)
                    {
                        
                    }
                    break;
                case "00048"://LPY 2015-3-25 09:57:01 添加 关闭显示客户端的测距、测面积功能
                    GlobalLayers.BufferGraphicLayer.ClearGraphics();
                    break;
                case "00049"://LPY 2015-4-1 13:21:57 添加 显示预案文本信息
                    //jsonForYATextPad = rejson;//LPY 2015-7-29 21:36:30 注释该段 因为不再使用纯文本预案详情，都改为HTML文档了
                    //GlobalLayers._yainfowindow.ShowYATextByJson();
                    //GlobalLayers._yainfowindow.Width = 2000;
                    //GlobalLayers._yainfowindow.Height = 1600;
                    try
                    {
                        //rejson["html"].ToString()标题名
                        GlobalLayers._yahtmlpad.strHtmlFileName = rejson["YAXQ"].ToString();
                        GlobalLayers._yahtmlpad.ShowHtmlByName();
                        //GlobalLayers._yahtmlpad.Width = 2000;//LPY 2015-12-22 19:23:11 暂时注释 因为素材没有找好
                        //GlobalLayers._yahtmlpad.Height = 1600;
                    }
                    catch (Exception)
                    {

                    }
                    break;
                case "00065"://民防主题菜单开启
                    //JObject js = rejson;
                    ClearAllVideos();//LPY 2016-6-1 修改 出现主菜单时，关掉屏幕上的视频
                    GlobalLayers.padMenu.BeginStoryboard(App.Current.FindResource("StoryboardForPadMenuShow") as Storyboard);
                    GlobalLayers.padMenu.DrawMenu2ByJson(rejson);
                    //LogHelper.WriteLog(typeof(MainWindow), rejson.ToString());
                    GlobalLayers._logoinfowindow.MF.Visibility = Visibility.Visible;
                    GlobalLayers._logoinfowindow.JW.Visibility = Visibility.Hidden;
                    
                    
                    break;
                case "00061"://民防主题菜单关闭（全部关闭）
                    //LogHelper.WriteLog(typeof(MainWindow), rejson.ToString());
                    //GlobalLayers.padMenu.Opacity = 0;
                    GlobalLayers.padMenu.BeginStoryboard(App.Current.FindResource("StoryboardForPadMenuClose") as Storyboard);
                    //LogHelper.WriteLog(typeof(MainWindow), "已执行：padMenu.Opacity = 0");
                    GlobalLayers._logoinfowindow.MF.Visibility = Visibility.Hidden;
                    GlobalLayers._logoinfowindow.JW.Visibility = Visibility.Visible;
                    //DropMenuAll();
                    
                    break;
                case "00064":
                    try
                    {
                        if (rejson["AttrStr"]["MC"]!=null)
                        {
                            GlobalLayers._skinfowindow.Tag = "水库详情-" + rejson["AttrStr"]["MC"].ToString();
                            switch (rejson["AttrStr"]["MC"].ToString())
                            {
                                case "秀岭水库":
                                    ControlSKInfoShow(1);
                                    break;
                                case "佛岭水库":
                                    ControlSKInfoShow(2);
                                    break;
                                case "西溪水库":
                                    ControlSKInfoShow(3);
                                    break;
                                default:
                                    break;
                            }
                        }
                        //ClearAllVideos();//CloseAllSPSPLD();//检查有无已打开的视频，并关闭
                        InitSPINFOPADSPLD();//
                        OpenVideoByJson(rejson["SPARR"] as JObject);
                    }
                    catch (Exception)
                    {}
                    break;
                case "00067"://关闭水库信息视频联动显示
                    try
                    {
                        GlobalLayers._skinfowindow.Width = 1;
                        GlobalLayers._skinfowindow.Height = 1;
                       // LogHelper.WriteLog(typeof(MainWindow), "完成水库信息视大小设置");


                        ClearAllVideos(); //CloseAllSPSPLD();//检查有无已打开的视频，并关闭
                       // LogHelper.WriteLog(typeof(MainWindow), "完成已打开视频全部关闭");
                        GlobalLayers.graphicsLayerSPLabel.Graphics.Clear();//清除旧视频位置标签
                       // LogHelper.WriteLog(typeof(MainWindow), "完成旧视频标签清除");
                        //ThicknessAnimation taOutSK = new ThicknessAnimation();
                        //taOutSK.From = new Thickness(0, 0, 0, 0);
                        //taOutSK.To = new Thickness(1700, 0, 0, 0);
                        //taOutSK.Duration = TimeSpan.FromSeconds(1);
                        //GlobalLayers._sppadSPLD.BeginAnimation(UserControl.BorderThicknessProperty, taOutSK);
                        //GlobalLayers._sppadSPLD.Margin = new Thickness(4800, 0, 0, 0);//LPY 2015-6-18 13:12:01 修改 让视频窗口背景板瞬间消失
                        //GlobalLayers._sppadSPLD.Width = 1;
                        //GlobalLayers._sppadSPLD.Height = 1;
                       // LogHelper.WriteLog(typeof(MainWindow), "完成视频联动视频背景板隐藏");
                        isOpenedSPLD = false;
                       // LogHelper.WriteLog(typeof(MainWindow), "设置IsOpened");
                        InitSPINFOPADSPLD();
                        //LogHelper.WriteLog(typeof(MainWindow), "完成初始化视频联动背景板初始化");
                        
                    }
                    catch (Exception)
                    {
                        
                    }
                    
                    break;
                case "00069"://LPY 2015-5-26 16:09:44 添加 打开通话窗口，并根据JSON显示通话人信息
                    //DoubleAnimation dTelIn = new DoubleAnimation();
                    //dTelIn.From=
                    try
                    {
                        if (rejson["BHXX"] != null)
                        {
                            telnum = rejson["BHXX"]["LXDH"].ToString();
                            GlobalLayers._telinfowidow.Width = 1600;
                            GlobalLayers._telinfowidow.Height = 1200;
                            //GlobalLayers._telinfowidow.Content = rejson["BHXX"] as JObject;
                            GlobalLayers._telinfowidow.spContent.DataContext = rejson["BHXX"] as JObject;
                            //GlobalLayers._telinfowidow.tbTalk.Visibility = Visibility.Collapsed;
                            //GlobalLayers._telinfowidow.tbDial.Visibility = Visibility.Collapsed;
                            //GlobalLayers._telinfowidow.tbHangup.Visibility = Visibility.Collapsed;
                        }
                    }
                    catch (Exception)
                    { }
                    break;
                case "00068"://LPY 2015-5-26 16:10:44 添加 根据JSON信息选择通话中或者挂断（关闭通话窗口）
                    try
                    {
                        if (rejson["STATE"] != null)
                        {
                            //message.Text = rejson.ToString();
                            if (telnum!=string.Empty)
                            {
                                message.Text = string.Format("{{\"CMD\":\"{0}\",\"STATE\":\"{1}\",\"TEL\":\"{2}\"}}",rejson["CMD"].ToString(),rejson["STATE"].ToString(),telnum);
                                prod.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                            }
                            //string telnum = (lobalLayers._telinfowidow.spContent.FindName("") as TextBlock).Text;
                            
                            //if (rejson["STATE"].ToString() == "dial")
                            //{
                                //GlobalLayers._telinfowidow.tbDial.Visibility = Visibility.Visible;
                                //GlobalLayers._telinfowidow.tbHangup.Visibility = Visibility.Collapsed;
                            //}
                            //else if (rejson["STATE"].ToString() == "hangUp")
                            //{
                                //GlobalLayers._telinfowidow.tbDial.Visibility = Visibility.Collapsed;
                                //GlobalLayers._telinfowidow.tbHangup.Visibility = Visibility.Visible;
                            //}
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case "00070":
                    try
                    {
                        telnum = string.Empty;
                        (GlobalLayers._telinfowidow.LayoutRoot.FindName("tbDial") as TextBlock).Text = "";
                        GlobalLayers._telinfowidow.Width = 1;
                        GlobalLayers._telinfowidow.Height = 1;
                        //GlobalLayers._telinfowidow.tbTalk.Visibility = Visibility.Collapsed;
                        //GlobalLayers._telinfowidow.tbDial.Visibility = Visibility.Collapsed;
                        //GlobalLayers._telinfowidow.tbHangup.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case "00071"://LPY 2015-7-9 14:15:25 
                    try
                    {
                        //rejson["html"].ToString()标题名
                        GlobalLayers._yahtmlpad.strHtmlFileName = rejson["HTML"].ToString();
                        GlobalLayers._yahtmlpad.ShowHtmlByName();
                        //GlobalLayers._yahtmlpad.Width = 2000;//LPY 2015-12-22 19:22:30 暂时注释 因为素材没有选好
                        //GlobalLayers._yahtmlpad.Height = 1600;
                    }
                    catch (Exception)
                    {
                        
                    }

                    break;
                case "00050"://LPY 2015-7-29 21:42:49 修改 把00050改到这里，因为做的是同一个动作。
                case "00072"://LPY 2015-7-9 14:15:25 
                    try
                    {
                        GlobalLayers._yahtmlpad.strHtmlFileName = string.Empty ;
                        GlobalLayers._yahtmlpad.Width = 1;
                        GlobalLayers._yahtmlpad.Height = 1;
                    }
                    catch (Exception)
                    {}
                    break;
                case "00074":
                    try
                    {
                        //先找出要放大的视频点，下墙并保存该视频点信息
                        //if (GlobalLayers._spinfowindow!=null)
                        //{
                        //    for (int i = 1; i <= 6; i++)
                        //    {
                        //        TextBlock tb = (TextBlock)GlobalLayers._spinfowindow.FindName("tb" + i.ToString());
                        //        if (tb.Tag.ToString() != "0")
                        //        { 
                        //            //sbTemp.AppendFormat("{{\"ID\":\"{0}\",\"STDID\":\"{1}\",\"MC\":\"{2}\"}},", tb.Text.Split('-')[0], tb.Tag,tb.Text.Split('-')[1]); 
                        //            if (rejson["DB33"].ToString()==tb.Tag.ToString())
                        //            {
                        //                oneVideoInfo.ClearInfo();
                        //                oneVideoInfo.ID = Convert.ToInt32(tb.Text.Split('-')[0]);
                        //                oneVideoInfo.MC = tb.Text.Split('-')[1];
                        //                oneVideoInfo.DB33 = tb.Tag.ToString();
                        //            }
                        //        }
                        //    }
                        //}
                        //if (oneVideoInfo.DB33!="")
                        //{
                        //    UpdateSPLabelOff(oneVideoInfo.ID.ToString(), oneVideoInfo.DB33);
                        //}

                        //CloseVideoZoomUp();
                        //OpenVideoZoomUp(rejson["DB33"].ToString());
                        string DB33 = rejson["DB33"].ToString();
                        if (DB33 != "")//发来的视频编号不为空
                        {
                            if (oneVideoInfo!=null)
                            {
                                MoveBackVideoZoomUp();
                            }

                            for (int i = 0; i < _b20controldata.Length; i++)
                            {
                                if (DB33==_b20controldata[i].DB33)
                                {
                                    VideoToWall._moveb20win(_b20controldata[i].winno, videoZoomUpX, videoZoomUpY, videoZoomUpWidth, videoZoomUpHeight);
                                    switch (i)
                                    {
                                        case 0:
                                            oneVideoInfo = new OneVideoInfo(DB33, x2, y1, w, h);
                                            break;
                                        case 1:
                                            oneVideoInfo = new OneVideoInfo(DB33, x2, y2, w, h);
                                            break;
                                        case 2:
                                            oneVideoInfo = new OneVideoInfo(DB33, x2, y3, w, h);
                                            break;
                                        case 3:
                                            oneVideoInfo = new OneVideoInfo(DB33, x1, y1, w, h);
                                            break;
                                        case 4:
                                            oneVideoInfo = new OneVideoInfo(DB33, x1, y2, w, h);
                                            break;
                                        case 5:
                                            oneVideoInfo = new OneVideoInfo(DB33, x1, y3, w, h);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            }
                            for (int i = 0; i < _b20controldataSPLD.Length; i++)
                            {
                                if (DB33 == _b20controldataSPLD[i].DB33)
                                {
                                    VideoToWall._moveb20win(_b20controldataSPLD[i].winno, videoZoomUpX, videoZoomUpY, videoZoomUpWidth, videoZoomUpHeight);
                                    switch (i)
                                    {
                                        case 0:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx3, SPLDy1, SPLDw, SPLDh);
                                            break;
                                        case 1:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx3, SPLDy2, SPLDw, SPLDh);
                                            break;
                                        case 2:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx2, SPLDy1, SPLDw, SPLDh);
                                            break;
                                        case 3:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx2, SPLDy2, SPLDw, SPLDh);
                                            break;
                                        case 4:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx1, SPLDy1, SPLDw, SPLDh);
                                            break;
                                        case 5:
                                            oneVideoInfo = new OneVideoInfo(DB33, SPLDx1, SPLDy2, SPLDw, SPLDh);
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                    break;
                case "00075":
                    try
                    {
                        //CloseVideoZoomUp();
                        //if (oneVideoInfo.DB33!="")
                        //{
                        //    UpdateSPLabel(oneVideoInfo.MC, oneVideoInfo.ID.ToString(), oneVideoInfo.DB33);
                        //}
                        MoveBackVideoZoomUp();
                    }
                    catch (Exception)
                    {
                        //_b20controldataZoomUp.DB33 = "";
                    }
                    break;
                case "00076":
                    //LPY 2015-8-11 19:47:42 添加 新增线路巡逻信息窗显示-隐藏显示
                    try
                    {
                        GlobalLayers._yaroadinfopad.Height = 1;
                        GlobalLayers._yaroadinfopad.Width = 1;
                    }
                    catch (Exception)
                    {}                    
                    break;
                default:
                    break;
            }
                    }));

        }
        #endregion

        //LPY 2015-8-19 18:45:29 添加 根据DB33编号，使已移动的视频回归位置
        private void MoveBackVideoZoomUp()
        {
            if (oneVideoInfo!=null)
            {
                if (oneVideoInfo.DB33!="")
                {
                    for (int i = 0; i < _b20controldata.Length; i++)
                    {
                        if (oneVideoInfo.DB33==_b20controldata[i].DB33)
                        {
                            VideoToWall._moveb20win(_b20controldata[i].winno, oneVideoInfo.x, oneVideoInfo.y, oneVideoInfo.w, oneVideoInfo.h);
                            oneVideoInfo = null;
                            break;
                        }
                    }
                    for (int i = 0; i < _b20controldataSPLD.Length; i++)
                    {
                        if (oneVideoInfo.DB33 == _b20controldataSPLD[i].DB33)
                        {
                            VideoToWall._moveb20win(_b20controldataSPLD[i].winno, oneVideoInfo.x, oneVideoInfo.y, oneVideoInfo.w, oneVideoInfo.h);
                            oneVideoInfo = null;
                            break;
                        }
                    }
                }
            }
        }

        //初始化民防应急疏散线路所需要的图层
        private void ClearMFLayers()
        {
            try
            {
                alPoints.Clear();
                //if (gsc!=null)
                //{
                //    gsc.alPoints.Clear();
                //}
                GeomServControl.alPoints.Clear();
                GeomServControl.alVideosPlayed.Clear();
                //如果存在graphicsLayerLine图层，先删除
                GraphicsLayer graphicsLayerLine_needoff = GlobalLayers._MainMap.Layers["graphicsLayerLine"] as GraphicsLayer;
                if (graphicsLayerLine_needoff != null)
                {
                    GlobalLayers._MainMap.Layers.Remove(graphicsLayerLine_needoff);
                }
                //如果存在graphicsLayerLinePointsRun图层，先删除
                GraphicsLayer graphicsLayerLinePointsRun_needoff = GlobalLayers._MainMap.Layers["graphicsLayerLinePointsRun"] as GraphicsLayer;
                if (graphicsLayerLinePointsRun_needoff != null)
                {
                    GlobalLayers._MainMap.Layers.Remove(graphicsLayerLinePointsRun_needoff);
                }

                //
                //GraphicsLayer graphicsLayerMove_needoff = GlobalLayers._MainMap.Layers["graphicsLayerMove"] as GraphicsLayer;
                //if (graphicsLayerMove_needoff!=null)
                //{
                //    GlobalLayers._MainMap.Layers.Remove(graphicsLayerMove_needoff);
                //}
            }
            catch (Exception)
            {
                
            }
            
        }

        //private void PointsRun()
        //{
        //    for (int i = 0; i < alPoints.Count; i++)
        //    {
        //        GlobalLayers._MainMap.Dispatcher.Invoke(
        //                   new Action(
        //                       delegate
        //                       {
        //                            GraphicsLayer graphicsLayerLinePointsRun = GlobalLayers._MainMap.Layers["graphicsLayerLinePointsRun"] as GraphicsLayer;
        //                            graphicsLayerLinePointsRun.Graphics.Clear();
        //                            graphicsLayerLinePointsRun.Graphics.Add((Graphic)alPoints[i]);
                                    
        //                       }));
        //        Thread.Sleep(1000);
        //    }
        //    threadPointsRun.Abort();
        //}

        //根据flagSPNum个数放置视频，改变视频背景名称
        private void UpdateSPLabel(string spName,string spid,string stdid)
        {
            int W = w;
            int H = h;
            //_b20controldata.ptr = Marshal.AllocHGlobal(128);
            if (GlobalLayers._spinfowindow.tb1.Tag.ToString()=="0")
            {
                GlobalLayers._spinfowindow.tb1.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb1.Tag = stdid;
                GlobalLayers._spinfowindow.tb1.Opacity = 1;
                _b20controldata[0].DB33 = stdid;
                _b20controldata[0].winno = VideoToWall._openb20win(stdid, x2, y1, W, H, _b20controldata[0].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb2.Tag.ToString()=="0")
            {
                GlobalLayers._spinfowindow.tb2.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb2.Tag = stdid;
                GlobalLayers._spinfowindow.tb2.Opacity = 1;
                _b20controldata[1].DB33 = stdid;
                _b20controldata[1].winno = VideoToWall._openb20win(stdid, x2, y2, W, H, _b20controldata[1].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb3.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb3.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb3.Tag = stdid;
                GlobalLayers._spinfowindow.tb3.Opacity = 1;
                _b20controldata[2].DB33 = stdid;
                _b20controldata[2].winno = VideoToWall._openb20win(stdid, x2, y3, W, H, _b20controldata[2].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb4.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb4.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb4.Tag = stdid;
                GlobalLayers._spinfowindow.tb4.Opacity = 1;
                _b20controldata[3].DB33 = stdid;
                _b20controldata[3].winno = VideoToWall._openb20win(stdid, x1, y1, W, H, _b20controldata[3].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb5.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb5.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb5.Tag = stdid;
                GlobalLayers._spinfowindow.tb5.Opacity = 1;
                _b20controldata[4].DB33 = stdid;
                _b20controldata[4].winno = VideoToWall._openb20win(stdid, x1, y2, W, H, _b20controldata[4].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb6.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb6.Text = spid + "." + spName;
                GlobalLayers._spinfowindow.tb6.Tag = stdid;
                GlobalLayers._spinfowindow.tb6.Opacity = 1;
                _b20controldata[5].DB33 = stdid;
                _b20controldata[5].winno = VideoToWall._openb20win(stdid, x1, y3, W, H, _b20controldata[5].ptr);
            }
            else
            {
                //多于6个，过多，抛弃
            }
        }

        private void UpdateSPLabelSPLD(string spName, string spid, string stdid)
        {
            int W = SPLDw;
            int H = SPLDh;
            //_b20controldata.ptr = Marshal.AllocHGlobal(128);
            if (GlobalLayers._sppadSPLD.tb1.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb1.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb1.Tag = stdid;
                GlobalLayers._sppadSPLD.tb1.Opacity = 1;
                _b20controldataSPLD[0].DB33 = stdid;
                _b20controldataSPLD[0].winno = VideoToWall._openb20win(stdid, SPLDx3, SPLDy1, W, H, _b20controldataSPLD[0].ptr);
            }
            else if (GlobalLayers._sppadSPLD.tb2.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb2.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb2.Tag = stdid;
                GlobalLayers._sppadSPLD.tb2.Opacity = 1;
                _b20controldataSPLD[1].DB33 = stdid;
                _b20controldataSPLD[1].winno = VideoToWall._openb20win(stdid, SPLDx3, SPLDy2, W, H, _b20controldataSPLD[1].ptr);
            }
            else if (GlobalLayers._sppadSPLD.tb3.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb3.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb3.Tag = stdid;
                GlobalLayers._sppadSPLD.tb3.Opacity = 1;
                _b20controldataSPLD[2].DB33 = stdid;
                _b20controldataSPLD[2].winno = VideoToWall._openb20win(stdid, SPLDx2, SPLDy1, W, H, _b20controldataSPLD[2].ptr);
            }
            else if (GlobalLayers._sppadSPLD.tb4.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb4.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb4.Tag = stdid;
                GlobalLayers._sppadSPLD.tb4.Opacity = 1;
                _b20controldataSPLD[3].DB33 = stdid;
                _b20controldataSPLD[3].winno = VideoToWall._openb20win(stdid, SPLDx2, SPLDy2, W, H, _b20controldataSPLD[3].ptr);
            }
            else if (GlobalLayers._sppadSPLD.tb5.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb5.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb5.Tag = stdid;
                GlobalLayers._sppadSPLD.tb5.Opacity = 1;
                _b20controldataSPLD[4].DB33 = stdid;
                _b20controldataSPLD[4].winno = VideoToWall._openb20win(stdid, SPLDx1, SPLDy1, W, H, _b20controldataSPLD[4].ptr);
            }
            else if (GlobalLayers._sppadSPLD.tb6.Tag.ToString() == "0")
            {
                GlobalLayers._sppadSPLD.tb6.Text = spid + "-" + spName;
                GlobalLayers._sppadSPLD.tb6.Tag = stdid;
                GlobalLayers._sppadSPLD.tb6.Opacity = 1;
                _b20controldataSPLD[5].DB33 = stdid;
                _b20controldataSPLD[5].winno = VideoToWall._openb20win(stdid, SPLDx1, SPLDy2, W, H, _b20controldataSPLD[5].ptr);
            }
            else
            {
                //多于6个，过多，抛弃
            }
        }

        //视频下窗的时候，背景板改变
        private void UpdateSPLabelOff(string spid,string stdid)
        {
            if (GlobalLayers._spinfowindow.tb6.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb6.Text = "";
                GlobalLayers._spinfowindow.tb6.Tag = "0";
                GlobalLayers._spinfowindow.tb6.Opacity = 0;
                if (_b20controldata[5].DB33!="")
                {
                    VideoToWall._closeb20win(_b20controldata[5].winno, _b20controldata[5].ptr);
                    _b20controldata[5].DB33 = "";
                }                
            }
            else if (GlobalLayers._spinfowindow.tb5.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb5.Text = "";
                GlobalLayers._spinfowindow.tb5.Tag = "0";
                GlobalLayers._spinfowindow.tb5.Opacity = 0;
                if (_b20controldata[4].DB33 != "")
                {
                    VideoToWall._closeb20win(_b20controldata[4].winno, _b20controldata[4].ptr);
                    _b20controldata[4].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb4.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb4.Text = "";
                GlobalLayers._spinfowindow.tb4.Tag = "0";
                GlobalLayers._spinfowindow.tb4.Opacity = 0;
                if (_b20controldata[3].DB33 != "")
                {
                    VideoToWall._closeb20win(_b20controldata[3].winno, _b20controldata[3].ptr);
                    _b20controldata[3].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb3.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb3.Text = "";
                GlobalLayers._spinfowindow.tb3.Tag = "0";
                GlobalLayers._spinfowindow.tb3.Opacity = 0;
                if (_b20controldata[2].DB33 != "")
                {
                    VideoToWall._closeb20win(_b20controldata[2].winno, _b20controldata[2].ptr);
                    _b20controldata[2].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb2.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb2.Text = "";
                GlobalLayers._spinfowindow.tb2.Tag = "0";
                GlobalLayers._spinfowindow.tb2.Opacity = 0;
                if (_b20controldata[1].DB33 != "")
                {
                    VideoToWall._closeb20win(_b20controldata[1].winno, _b20controldata[1].ptr);
                    _b20controldata[1].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb1.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb1.Text = "";
                GlobalLayers._spinfowindow.tb1.Tag = "0";
                GlobalLayers._spinfowindow.tb1.Opacity = 0;
                if (_b20controldata[0].DB33 != "")
                {
                    VideoToWall._closeb20win(_b20controldata[0].winno, _b20controldata[0].ptr);
                    _b20controldata[0].DB33 = "";
                }
            }
        }

        //初始化视频窗背景板
        private void InitSPINFOPAD()
        {
            GlobalLayers._spinfowindow.tb1.Text = "";
            GlobalLayers._spinfowindow.tb1.Tag = "0";
            GlobalLayers._spinfowindow.tb2.Text = "";
            GlobalLayers._spinfowindow.tb2.Tag = "0";
            GlobalLayers._spinfowindow.tb3.Text = "";
            GlobalLayers._spinfowindow.tb3.Tag = "0";
            GlobalLayers._spinfowindow.tb4.Text = "";
            GlobalLayers._spinfowindow.tb4.Tag = "0";
            GlobalLayers._spinfowindow.tb5.Text = "";
            GlobalLayers._spinfowindow.tb5.Tag = "0";
            GlobalLayers._spinfowindow.tb6.Text = "";
            GlobalLayers._spinfowindow.tb6.Tag = "0";
            GlobalLayers._spinfowindow.tb1.Opacity = 0;
            GlobalLayers._spinfowindow.tb2.Opacity = 0;
            GlobalLayers._spinfowindow.tb3.Opacity = 0;
            GlobalLayers._spinfowindow.tb4.Opacity = 0;
            GlobalLayers._spinfowindow.tb5.Opacity = 0;
            GlobalLayers._spinfowindow.tb6.Opacity = 0;
        }

        private void InitSPINFOPADSPLD()
        {
            GlobalLayers._sppadSPLD.tb1.Text = "";
            GlobalLayers._sppadSPLD.tb1.Tag = "0";
            GlobalLayers._sppadSPLD.tb2.Text = "";
            GlobalLayers._sppadSPLD.tb2.Tag = "0";
            GlobalLayers._sppadSPLD.tb3.Text = "";
            GlobalLayers._sppadSPLD.tb3.Tag = "0";
            GlobalLayers._sppadSPLD.tb4.Text = "";
            GlobalLayers._sppadSPLD.tb4.Tag = "0";
            GlobalLayers._sppadSPLD.tb5.Text = "";
            GlobalLayers._sppadSPLD.tb5.Tag = "0";
            GlobalLayers._sppadSPLD.tb6.Text = "";
            GlobalLayers._sppadSPLD.tb6.Tag = "0";
            GlobalLayers._sppadSPLD.tb1.Opacity = 0;
            GlobalLayers._sppadSPLD.tb2.Opacity = 0;
            GlobalLayers._sppadSPLD.tb3.Opacity = 0;
            GlobalLayers._sppadSPLD.tb4.Opacity = 0;
            GlobalLayers._sppadSPLD.tb5.Opacity = 0;
            GlobalLayers._sppadSPLD.tb6.Opacity = 0;
        }

        //关闭所有已打开的视频
        private void CloseAllSP()
        {
            for (int i = 0; i < _b20controldata.Length; i++)
            {
                if (_b20controldata[i].DB33!="")//存在视频，关闭
                {
                    VideoToWall._closeb20win(_b20controldata[i].winno, _b20controldata[i].ptr);
                    _b20controldata[i].DB33 = "";
                    //LogHelper.WriteLog(typeof(MainWindow), "CloseAllSP执行一次");
                }
            }
            ThicknessAnimation taOut = new ThicknessAnimation();
            taOut.From = new Thickness(0, 0, 0, 0);
            taOut.To = new Thickness(1700, 0, 0, 0);
            taOut.Duration = TimeSpan.FromSeconds(0);
            GlobalLayers._spinfowindow.BeginAnimation(UserControl.BorderThicknessProperty, taOut);
        }

        private void CloseAllSPSPLD()
        {
            for (int i = 0; i < _b20controldataSPLD.Length; i++)
            {
                if (_b20controldataSPLD[i].DB33 != "")//存在视频，关闭
                {
                    VideoToWall._closeb20win(_b20controldataSPLD[i].winno, _b20controldataSPLD[i].ptr);
                    _b20controldataSPLD[i].DB33 = "";
                    //LogHelper.WriteLog(typeof(MainWindow), "CloseAllSPSPLD执行一次");
                }
            }
            //VideoToWall._closeallb20win();
            ThicknessAnimation taIn = new ThicknessAnimation();
            taIn.From = new Thickness(0, 0, 0, 0);
            taIn.To = new Thickness(4800, 0, 0, 0);
            taIn.Duration = TimeSpan.FromSeconds(0);
            GlobalLayers._sppadSPLD.BeginAnimation(UserControl.BorderThicknessProperty, taIn);
        }

        //LPY 2016-5-4 添加 关闭屏幕上所有已打开的视频
        private void CloseSPXLVideo()
        {
            if (QueryLayer._currentVideo.DB33 != "")
            {
                VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
            }
        }

        private void ClearAllVideos()
        {
            CloseAllSP();
            
            CloseAllSPSPLD();
            

            InitSPINFOPAD(); 
            InitSPINFOPADSPLD();

            CloseSPXLVideo();
        }


        //根据发来的JSON串组合成轨迹线路
        private string GetGuiJi(string scr)
        {
            StringBuilder sbReturn = new StringBuilder();

            StringBuilder sbTmp = new StringBuilder();
            JArray jar = JArray.Parse(scr);
            if (jar.Count>0)
            {
                for (int i = 0; i < jar.Count-1; i++)
                {
                    JObject j = JObject.Parse(jar[i].ToString());
                    sbTmp.AppendFormat("[{0},{1}],", j["X"].ToString(), j["Y"].ToString());
                }
                sbTmp.AppendFormat("[{0},{1}]", JObject.Parse(jar[jar.Count - 1].ToString())["X"].ToString(), JObject.Parse(jar[jar.Count - 1].ToString())["Y"].ToString());
            }            
            string spatial = "\"spatialReference\":{\"wkid\":4326}";
            sbReturn.AppendFormat("{{\"paths\":[[{0}]],{1}}}",sbTmp.ToString(),spatial);
            return sbReturn.ToString();
        }

        //根据Geometry反转线路
        private Geometry ReverseGeometry(Geometry g)
        {
            JObject obj = JObject.Parse(g.ToJson());
            JArray jar = JArray.Parse(obj["paths"].ToString());
            jar = (JArray)jar[0];

            int i = 0;
            int max = jar.Count - 1;
            while (i!=jar.Count/2)
            {
                JToken temp = jar[i];
                jar[i] = jar[max];
                jar[max] = temp;
                if (i!=max)
                {
                    i++;
                    max--;
                }
            }
            string strJar = jar.ToString().Replace("{","").Replace("}","");
            string scrGeo = "{\"geometry\":{\"spatialReference\":{\"wkid\":4326},\"paths\":[" + strJar + "]}}";
            Geometry _g = GeometryFunc.ParsefromJson(JObject.Parse(scrGeo));
            return _g;
        }

        //LPY 2015-5-21 17:23:29 添加 根据JSON数据，绘制大屏上的图标
        private void DrawIconsLevel1(JObject json)
        {
            try
            {
                JArray jar = json["SPARR"] as JArray;
                for (int i = 0; i < jar.Count; i++)
                {
                    Label lb = new Label();
                    lb.Name = jar[i]["YAMC"].ToString();
                    lb.Tag = "1";
                    lb.ContentTemplate = GlobalLayers.LayoutRoot.Resources["ButtonTemplateBase"] as DataTemplate;
                    lb.Content = jar[i];
                    //lb.Opacity = 0;
                    lb.Margin = new Thickness(1670 + i * 800, 0, 0, 0);
                    lb.VerticalAlignment = VerticalAlignment.Bottom;

                    DoubleAnimation daOpacity = new DoubleAnimation()
                    {
                        From = 0,
                        To = 1,
                        Duration = TimeSpan.FromSeconds(0.5),
                        FillBehavior = FillBehavior.HoldEnd
                    };
                    lb.BeginAnimation(UserControl.OpacityProperty, daOpacity);
                    GlobalLayers.LayoutRoot.Children.Add(lb);
                }
            }
            catch (Exception)
            {}            
        }

        //LPY 2015-5-21 17:23:29 添加 根据JSON数据，绘制大屏上的图标-2级菜单图标
        private void DrawIconsLevel2(JObject json)
        {
            try
            {
                ////////////////////////
                double dMenuLevel2MarginLeft = 50;
                if (json["LEVEL"].ToString()=="2")
                {
                    for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
                    {
                        if (typeof(Label)==GlobalLayers.LayoutRoot.Children[i].GetType())
                        {
                            if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString()=="1"&&JObject.Parse(((Label)GlobalLayers.LayoutRoot.Children[i]).Content.ToString())["YAMC"].ToString()==json["MENU"].ToString())
                            {
                                dMenuLevel2MarginLeft = ((Label)GlobalLayers.LayoutRoot.Children[i]).Margin.Left;
                            }
                        }
                    }
                }
                ////////////////////////


                JArray jar = json["SPARR"] as JArray;
                for (int i = 0; i < jar.Count; i++)
                {
                    Label lb = new Label();
                    lb.Name = jar[i]["YAMC"].ToString();
                    lb.Tag = "2";
                    lb.ContentTemplate = GlobalLayers.LayoutRoot.Resources["Level2TemplateBase"] as DataTemplate;
                    lb.Content = jar[i];
                    lb.Opacity = 0;
                    //lb.Margin = new Thickness(50 , 200+i*180, 0, 0);
                    ////////////////
                    lb.Margin = new Thickness(dMenuLevel2MarginLeft, 0, 0, 200 + i * 180);
                    lb.VerticalAlignment = VerticalAlignment.Bottom;
                    ////////////////////

                    DoubleAnimation daOpacity = new DoubleAnimation() { 
                        From=0,To=1,BeginTime=TimeSpan.FromSeconds(Convert.ToDouble(i)/4),Duration=TimeSpan.FromSeconds(0.25),FillBehavior=FillBehavior.HoldEnd
                    };
                    lb.BeginAnimation(UserControl.OpacityProperty, daOpacity);
                    GlobalLayers.LayoutRoot.Children.Add(lb);
                    
                }
            }
            catch (Exception)
            { }
        }

        

        //LPY 2015-5-21 19:53:25 添加 根据菜单级别和主题名，改变菜单背景色，突出
        private void ChangeMenuColorLevel1(JObject json)
        {
            if (json["LEVEL"].ToString() == "2")
            {
                for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
                {
                    if (typeof(Label) == GlobalLayers.LayoutRoot.Children[i].GetType())
                    {
                        //GlobalLayers.LayoutRoot.Children.Remove(GlobalLayers.LayoutRoot.Children[i]);
                        if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString() == "1")
                        {
                            ((Label)GlobalLayers.LayoutRoot.Children[i]).BeginAnimation(UserControl.OpacityProperty, null);
                            if (JObject.Parse(((Label)GlobalLayers.LayoutRoot.Children[i]).Content.ToString())["YAMC"].ToString() == json["MENU"].ToString())
                            {
                                ((Label)GlobalLayers.LayoutRoot.Children[i]).ContentTemplate = GlobalLayers.LayoutRoot.Resources["ButtonTemplateSpecial"] as DataTemplate;
                                //((Label)GlobalLayers.LayoutRoot.Children[i]).Content = json[""] as JObject;
                                DoubleAnimation daOpacity = new DoubleAnimation();
                                daOpacity.From = 1;
                                daOpacity.To = 0.4;
                                daOpacity.Duration = TimeSpan.FromSeconds(0.25);
                                daOpacity.RepeatBehavior = new RepeatBehavior(2);
                                daOpacity.AutoReverse = true;
                                ((Label)GlobalLayers.LayoutRoot.Children[i]).BeginAnimation(UserControl.OpacityProperty, daOpacity);//添加动画                                
                            }
                        }
                        
                    }
                }
            }
        }

        private void ChangeMenuColorLevel2(JObject json)
        {
            try
            {
                ArrayList alMenuLevel2 = new ArrayList();
                alMenuLevel2.Clear();
                double AnimationTime = Convert.ToDouble(ConfigurationManager.AppSettings["AnimationTime"].ToString());

                if (json["LEVEL"].ToString() == "3")
                {
                    for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
                    {
                        if (typeof(Label) == GlobalLayers.LayoutRoot.Children[i].GetType())
                        {
                            //GlobalLayers.LayoutRoot.Children.Remove(GlobalLayers.LayoutRoot.Children[i]);
                            if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString() == "2")
                            {
                                alMenuLevel2.Add((Label)GlobalLayers.LayoutRoot.Children[i]);
                                ((Label)GlobalLayers.LayoutRoot.Children[i]).BeginAnimation(UserControl.OpacityProperty, null);//删除动画
                                if (JObject.Parse(((Label)GlobalLayers.LayoutRoot.Children[i]).Content.ToString())["YAMC"].ToString() == json["MENU"].ToString())
                                {
                                    ((Label)GlobalLayers.LayoutRoot.Children[i]).ContentTemplate = GlobalLayers.LayoutRoot.Resources["Level2TemplateSpecial"] as DataTemplate;
                                    ((Label)GlobalLayers.LayoutRoot.Children[i]).Opacity = 0;
                                    DoubleAnimation daOpacity = new DoubleAnimation();
                                    daOpacity.From = 1;
                                    daOpacity.To = 0.5;
                                    daOpacity.Duration = TimeSpan.FromSeconds(AnimationTime);
                                    daOpacity.RepeatBehavior = new RepeatBehavior(1);
                                    daOpacity.AutoReverse = true;
                                    daOpacity.FillBehavior = FillBehavior.Stop;
                                    ((Label)GlobalLayers.LayoutRoot.Children[i]).BeginAnimation(UserControl.OpacityProperty, daOpacity);//添加动画

                                }
                                else
                                {
                                    ((Label)GlobalLayers.LayoutRoot.Children[i]).Opacity = 0;
                                    DoubleAnimation daOpacity = new DoubleAnimation();
                                    daOpacity.From = 1;
                                    daOpacity.To = 0.1;
                                    daOpacity.AccelerationRatio = 0.9; daOpacity.DecelerationRatio = 0.05;
                                    daOpacity.Duration = TimeSpan.FromSeconds(AnimationTime * 2);
                                    //daOpacity.RepeatBehavior = new RepeatBehavior(2);
                                    //daOpacity.AutoReverse = true;
                                    daOpacity.FillBehavior = FillBehavior.Stop;
                                    ((Label)GlobalLayers.LayoutRoot.Children[i]).BeginAnimation(UserControl.OpacityProperty, daOpacity);//添加动画

                                }
                            }    
                        }
                    }
                    //for (int j = alMenuLevel2.Count - 1; j >= 0; j--)
                    //{
                    //    DoubleAnimation daOpacity = new DoubleAnimation()
                    //    {
                    //        From = 1,
                    //        To = 0,
                    //        BeginTime = TimeSpan.FromSeconds(1 + Convert.ToDouble(( j/4))),
                    //        Duration = TimeSpan.FromSeconds(0.25),
                    //        FillBehavior = FillBehavior.HoldEnd
                    //    };
                    //    ((Label)alMenuLevel2[j]).BeginAnimation(UserControl.OpacityProperty, daOpacity);
                    //}
                }
            }
            catch (Exception)
            {
            }
            
        }

        //关闭所有菜单
        private void DropMenuAll()
        {
            try
            {
                ArrayList alNeedToDropLabel = new ArrayList();
                for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
                {
                    if (typeof(Label) == GlobalLayers.LayoutRoot.Children[i].GetType())
                    {
                        //GlobalLayers.LayoutRoot.Children.Remove(GlobalLayers.LayoutRoot.Children[i]);
                        alNeedToDropLabel.Add(GlobalLayers.LayoutRoot.Children[i]);
                    }
                }
                for (int j = 0; j < alNeedToDropLabel.Count; j++)
                {
                    GlobalLayers.LayoutRoot.Children.Remove((Label)alNeedToDropLabel[j]);
                }
                alNeedToDropLabel.Clear();
            }
            catch (Exception)
            {

            }
        }

        //关闭2级菜单
        private void DropMenuLevel2()
        {
            try
            {
                ArrayList alNeedToDropLabel = new ArrayList();
                for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
                {
                    if (typeof(Label) == GlobalLayers.LayoutRoot.Children[i].GetType())
                    {
                        //GlobalLayers.LayoutRoot.Children.Remove(GlobalLayers.LayoutRoot.Children[i]);
                        if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString()=="2")
                        {
                            alNeedToDropLabel.Add(GlobalLayers.LayoutRoot.Children[i]);
                        }                        
                    }
                }
                for (int j = 0; j < alNeedToDropLabel.Count; j++)
                {
                    GlobalLayers.LayoutRoot.Children.Remove((Label)alNeedToDropLabel[j]);
                }
                alNeedToDropLabel.Clear();
            }
            catch (Exception)
            {
            }
        }

        //所有菜单恢复普通底色
        private void InitMenuColorAll(string level)
        {
            for (int i = 0; i < GlobalLayers.LayoutRoot.Children.Count; i++)
            {
                if (typeof(Label) == GlobalLayers.LayoutRoot.Children[i].GetType())
                {
                    if (level=="3")
                    {
                        if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString() == "2")
                        {
                            ((Label)GlobalLayers.LayoutRoot.Children[i]).ContentTemplate = GlobalLayers.LayoutRoot.Resources["Level2TemplateBase"] as DataTemplate;
                        }
                    }
                    else
                    {
                        if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString() == "1")
                        {
                            ((Label)GlobalLayers.LayoutRoot.Children[i]).ContentTemplate = GlobalLayers.LayoutRoot.Resources["ButtonTemplateBase"] as DataTemplate;

                        }
                        else if (((Label)GlobalLayers.LayoutRoot.Children[i]).Tag.ToString() == "2")
                        {
                            ((Label)GlobalLayers.LayoutRoot.Children[i]).ContentTemplate = GlobalLayers.LayoutRoot.Resources["Level2TemplateBase"] as DataTemplate;
                        }
                    }                    
                }
            }
        }

        //LPY 2015-5-22 21:16:13 添加 控制各个水库信息的显隐
        private void ControlSKInfoShow(int index)
        {
            GlobalLayers._skinfowindow.Width = 3200;
            GlobalLayers._skinfowindow.Height = 2400;

            GlobalLayers._skinfowindow.gridSK1.Visibility = Visibility.Hidden;
            GlobalLayers._skinfowindow.gridSK2.Visibility = Visibility.Hidden;
            GlobalLayers._skinfowindow.gridSK3.Visibility = Visibility.Hidden;
            switch (index)
            {
                case 1:
                    GlobalLayers._skinfowindow.gridSK1.Visibility = Visibility.Visible;
                    break;
                case 2:
                    GlobalLayers._skinfowindow.gridSK2.Visibility = Visibility.Visible;
                    break;
                case 3:
                    GlobalLayers._skinfowindow.gridSK3.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        //LPY 2015-5-23 08:56:09 添加 根据点击水库、避灾点等信息发来的视频组，打开相应的视频
        private void OpenVideoByJson(JObject json)
        {
            JArray jar = json["aaData"] as JArray;
            GlobalLayers.graphicsLayerSPLabel.Graphics.Clear();//清除旧视频位置标签
            try
            {
                //flagSPNum = 0;//初始化视频打开个数计数器
                ClearAllVideos();//CloseAllSPSPLD();//检查有无已打开的视频，并关闭
                if (!isOpenedSPLD)//已打开状态下，不关闭
                {
                    GlobalLayers._sppadSPLD.Width = 4800;
                    GlobalLayers._sppadSPLD.Height = 2400;
                    //GlobalLayers._sppadSPLD.Margin = new Thickness(0);
                    ThicknessAnimation taIn = new ThicknessAnimation();
                    taIn.From = new Thickness(4800, 0, 0, 0);
                    taIn.To = new Thickness(0, 0, 0, 0);
                    taIn.Duration = TimeSpan.FromSeconds(0);
                    GlobalLayers._sppadSPLD.BeginAnimation(UserControl.BorderThicknessProperty, taIn);
                    isOpenedSPLD = true;
                }
            }
            catch (Exception)
            {}

            try
            {
                GlobalLayers._MainMap.Dispatcher.BeginInvoke(new Action(() => {
                    //Thread.Sleep(2000);
                    for (int i = 0; i < jar.Count; i++)
                    {
                        //以下几行添加视频位置标签在地图上
                        Graphic graphicSPLabel = new Graphic();
                        graphicSPLabel.Geometry = new MapPoint(Convert.ToDouble(jar[i]["X"].ToString()), Convert.ToDouble(jar[i]["Y"].ToString()), new SpatialReference(4326));
                        graphicSPLabel.Symbol = App.Current.Resources["SPNum"] as Symbol;
                        graphicSPLabel.Attributes["ID"] = (i+1).ToString();
                        graphicSPLabel.Attributes["STDID"] = jar[i]["DB33"].ToString();
                        graphicSPLabel.SetZIndex(100);
                        GlobalLayers.graphicsLayerSPLabel.Graphics.Add(graphicSPLabel);

                        UpdateSPLabelSPLD(jar[i]["SPMC"].ToString(), (i + 1).ToString(), jar[i]["DB33"].ToString());
                    }
                }));
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// LPY 2015-8-12 10:39:25 添加
        /// 根据数字控制民防避灾点图层的显隐
        /// </summary>
        private void ShowMFBZDByNum(int num)
        {
            //LogHelper.WriteLog(typeof(MainWindow), "执行ShowMFBZDByNum");
            try
            {
                CloseMFBZDShow();
                ClosePlotsShow();
                switch (num)
                {
                    case 0:
                        GraphicsLayer glLine1 = GlobalLayers._MainMap.Layers["glMFBZDLine1"] as GraphicsLayer;  glLine1.Visible = true;//LogHelper.WriteLog(typeof(MainWindow),glLine1.Visible.ToString()+"-2316");
                        GraphicsLayer glPlot1 = GlobalLayers._MainMap.Layers["glPlot1"] as GraphicsLayer;  glPlot1.Visible=true;
                        GlobalLayers._yaroadinfopad.tbRoad.Text = "院桥街至岙里街村疏散路线";
                        break;
                    case 1:
                        GraphicsLayer glLine2 = GlobalLayers._MainMap.Layers["glMFBZDLine2"] as GraphicsLayer;  glLine2.Visible = true;
                        GraphicsLayer glPlot2 = GlobalLayers._MainMap.Layers["glPlot2"] as GraphicsLayer;  glPlot2.Visible=true;
                        GlobalLayers._yaroadinfopad.tbRoad.Text = "下抱至繁荣村疏散路线";
                        break;
                    case 2:
                        GraphicsLayer glLine3 = GlobalLayers._MainMap.Layers["glMFBZDLine3"] as GraphicsLayer;  glLine3.Visible = true;
                        GraphicsLayer glPlot3 = GlobalLayers._MainMap.Layers["glPlot3"] as GraphicsLayer;  glPlot3.Visible=true;
                        GlobalLayers._yaroadinfopad.tbRoad.Text = "前庄至占堂村疏散路线";
                        break;
                    default:
                        break;
                }
                //GraphicsLayer glLine1 = GlobalLayers._MainMap.Layers["glMFBZDLine1"] as GraphicsLayer;  glLine1.Visible = false;
                //GraphicsLayer glLine2 = GlobalLayers._MainMap.Layers["glMFBZDLine2"] as GraphicsLayer;  glLine2.Visible = false;
                //GraphicsLayer glLine3 = GlobalLayers._MainMap.Layers["glMFBZDLine3"] as GraphicsLayer;  glLine3.Visible = false;
            }
            catch (Exception)
            {                
            }
        }

        private void CloseMFBZDShow()
        {
            try
            {
                GraphicsLayer glLine1 = GlobalLayers._MainMap.Layers["glMFBZDLine1"] as GraphicsLayer; glLine1.Visible = false;
                GraphicsLayer glLine2 = GlobalLayers._MainMap.Layers["glMFBZDLine2"] as GraphicsLayer; glLine2.Visible = false;
                GraphicsLayer glLine3 = GlobalLayers._MainMap.Layers["glMFBZDLine3"] as GraphicsLayer; glLine3.Visible = false;
            }
            catch (Exception)
            {
            }
        }

        private void ClosePlotsShow()
        {
            GraphicsLayer glPlot1 = GlobalLayers._MainMap.Layers["glPlot1"] as GraphicsLayer; glPlot1.Visible = false;
            GraphicsLayer glPlot2 = GlobalLayers._MainMap.Layers["glPlot2"] as GraphicsLayer; glPlot2.Visible = false;
            GraphicsLayer glPlot3 = GlobalLayers._MainMap.Layers["glPlot3"] as GraphicsLayer; glPlot3.Visible = false;
        }

        private void ShowMFLines()
        {
            ClearMFLayers();
            //int num = -1;

            FeatureLayer oneLine = GlobalLayers._MainMap.Layers["LINE_PL"] as FeatureLayer;
            
            //动态线条，显示用，关闭视频巡逻时要关闭
            GraphicsLayer graphicsLayerMove = new GraphicsLayer();
            graphicsLayerMove.ID = "graphicsLayerMove";
            graphicsLayerMove.Visible = true;
            GlobalLayers._MainMap.Layers.Add(graphicsLayerMove);

            for (int i = 0; i < 3; i++)
            {
                Graphic _g = new Graphic();
                _g = oneLine.Graphics[i] as Graphic;
                Graphic moveLine = new Graphic()
                {
                    Symbol = App.Current.Resources["MFYALine1"] as Symbol,//动态线条
                    Geometry = ReverseGeometry(_g.Geometry)//_g.Geometry //ReverseGeometry(_g.Geometry)
                };
                graphicsLayerMove.Graphics.Add(moveLine);
            }
            

            //ShowMFBZDByNum(num);
        }
        
        

        #region  根据指令控制局部地图窗口
        private string createmapwindow(JObject jsoncommand)
        {
            try
            {
                string pName = jsoncommand["WINID"].ToString();

                if (GlobalLayers.LayoutRoot.FindName(pName) == null)
                {
                    mapwindows mapwindow = new mapwindows();
                    MapPoint pPoint = new MapPoint((double)jsoncommand["CENTX"], (double)jsoncommand["CENTY"]);
                    double pointx = (double)jsoncommand["CENTX"];
                    double pointy = (double)jsoncommand["CENTY"];
                    mapwindow.map.Extent = new Envelope(pointx - 0.0000035691915087454205 * 640, pointy - 0.0000035691915087454205 * 480, pointx + 0.0000035691915087454205 * 640, pointy + 0.0000035691915087454205 * 480);

                    mapwindow.VerticalAlignment = VerticalAlignment.Top;
                    mapwindow.HorizontalAlignment = HorizontalAlignment.Left;
                    mapwindow.Width = 800;
                    mapwindow.Height = 600;
                    mapwindow.Margin = new Thickness(600, 200, 0, 0);
                    mapwindow.Name = pName;
                    mapwindow.lbsTitle.Content = pName;
                    MW.RegisterName(pName, mapwindow);
                    GlobalLayers.LayoutRoot.Children.Add(mapwindow);
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
                mapwindows mapwindow = GlobalLayers.LayoutRoot.FindName(pName) as mapwindows;
                if (mapwindow != null)
                {
                    GlobalLayers.LayoutRoot.Children.Remove(mapwindow);
                    MW.UnregisterName(pName);   //add by lzd
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
        #endregion


        #region 根据指令控制信息窗口窗口
        private string openwindow(JObject jsoncommand)
        {
            try
            {
                string windowID = jsoncommand["WID"].ToString();
                MapPoint pPoint = new MapPoint((double)jsoncommand["X"], (double)jsoncommand["Y"]);
                string wbsURL = jsoncommand["URL"].ToString();

                infowindows pinfowindow = new infowindows();
                pinfowindow.Margin = new Thickness(20);
                pinfowindow.VerticalAlignment = VerticalAlignment.Top;
                pinfowindow.HorizontalAlignment = HorizontalAlignment.Left;
                pinfowindow.Width = 1030;
                pinfowindow.Height = 770;
                pinfowindow.wbs.Navigate(wbsURL);
                pinfowindow.lbsTitle.Content = windowID;

                pinfowindow.Name = windowID;
                MW.RegisterName(windowID, pinfowindow);
                GlobalLayers.LayoutRoot.Children.Add(pinfowindow);
                return windowID;
            }
            catch (Exception)
            {
                return "false";

            }
        }

        private string closewindow(JObject jsoncommand)
        {
            try
            {
                string pName = jsoncommand["WID"].ToString();
                infowindows pinfowindow = GlobalLayers.LayoutRoot.FindName(pName) as infowindows;
                if (pinfowindow != null)
                {

                    GlobalLayers.LayoutRoot.Children.Remove(pinfowindow);
                    MW.UnregisterName(pName);   //add by lzd
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
        #endregion

    }

    class OneVideoInfo
    {
        public int ID;
        public string MC;
        public string DB33;
        public int x;
        public int y;
        public int h;
        public int w;

        public OneVideoInfo()
        {
            ID = 0;
            MC = "";
            DB33 = "";
            x = 0;
            y = 0;
            h = 0;
            w = 0;
        }

        public OneVideoInfo(string _db33, int _x, int _y, int _h, int _w)
        {
            DB33 = _db33;
            x = _x;
            y = _y;
            h = _h;
            w = _w;
        }

        public void ClearInfo()
        {
            ID = 0;
            MC = "";
            DB33 = "";
            x = 0;
            y = 0;
            h = 0;
            w = 0;
        }
    }
}
