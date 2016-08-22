using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

namespace zsdpmap.classes
{
    class QueryLayer
    {
        string LayerName;
        string FeatureName;
        Geometry gs;
        QueryTask _queryTask;
        //public static ArrayList alVideos = new ArrayList();
        //public static ArrayList alVideosPlayed = new ArrayList();
        public static VideoToWall.B20ControlData _currentVideo = new VideoToWall.B20ControlData();
        public static int scrX = 0;
        public static int scrY = 0;
        public static string lastPlayedid = "";
        public static bool haveOpened = false;
        private static int shiftX = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["shiftX"]);
        private static int shiftY = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["shiftY"]);

        public QueryLayer(Geometry Gsearch,string _FeatureName,string _LayerName)
        {
            //  _FeatureName;信息图层名
            FeatureName = _FeatureName;
            LayerName = _LayerName;     // 显示图层
            gs = Gsearch;               // 搜索的区域
            //alVideosPlayed.Clear();
            //scrX = 0;
            //scrY = 0;
            //lastPlayedid = "";
            _queryTask = new QueryTask(System.Configuration.ConfigurationManager.AppSettings[_FeatureName].ToString());
            _queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
            _queryTask.Failed += QueryTask_Failed;
        }

        public void Do()
        {
            Query _query = new Query();
            _query.ReturnGeometry = true;
            _query.OutSpatialReference = new SpatialReference(4326);
            _query.Geometry = gs;
            _query.OutFields.Add("*");
            _queryTask.ExecuteAsync(_query);
        }

        private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            try
            {
                FeatureSet featureSet = args.FeatureSet;

                if (featureSet == null || featureSet.Features.Count == 0)   // 未扫到视频
                {
                    return;
                }
    
                if (featureSet.Features.Count > 0)
                {
                   //foreach (Graphic feature in featureSet.Features)
                   //{
   // GlobalLayers._MainMap.Dispatcher.Invoke(
   // new Action(
   // delegate
   // {
                    
                    Graphic feature = featureSet.Features[0];//只取查询到的摄像头集合的第一个来播
                    string stdid = feature.Attributes["STDID"].ToString();//获取视频编号，用来播放视频
                    //if (!GeomServControl.alVideoNeedToPlay.Contains(stdid))//LPY 2015-3-13 17:00:45 添加 用来修改为另外一种方式显示视频点上墙排版，但是效果不好，暂时注释掉
                    //{
                    //    GeomServControl.alVideoNeedToPlay.Add(stdid);
                    //    return;
                    //}
                    //return;
                    if (GeomServControl.alVideosPlayed.Contains(stdid))//首个视频编号已存在于已播放列表，不需要重新打开，其实也不用打开列表
                    {
                        //_currentVideo.DB33 = "";     // 不重新打开不能把当前的视频号清除掉
                        //scrX = 0;
                        //scrY = 0;
                        return;
                    }
                    //if (_currentVideo.DB33 != "")
                    //{
                    //    VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                    //    //QueryLayer.lastPlayedid = "";
                    //    _currentVideo.DB33 = "";
                    //}
                    double x = Convert.ToDouble(feature.Attributes["X"].ToString());//地图X
                    double y = Convert.ToDouble(feature.Attributes["Y"].ToString());
                    MapPoint mapPoint = new MapPoint(x, y);
                    mapPoint.SpatialReference = new SpatialReference(4326);
                    Point scrPoint = GlobalLayers._MainMap.MapToScreen(mapPoint);//将地图坐标转换为屏幕坐标，取屏幕x,y作为视频开窗起始点
                    
                    _currentVideo.DB33 = stdid;//待播放的视频ID
                    lastPlayedid = stdid;//同时把待播放的视频ID记录为播放下一个视频前需要关闭的视频编号

                    int videoX = Convert.ToInt32(scrPoint.X);
                    videoX = videoX < 0 ? 0 : videoX;//坐标小于0的话，取0值
                    scrX = Convert.ToInt32(videoX * 1.2) - shiftX;//这里的1.2和下面Y坐标乘以1.6是因为大屏分辨率的问题，可能还需调整
                    scrX = scrX > 0 ? scrX : 0;

                    int videoY = Convert.ToInt32(scrPoint.Y);
                    videoY = videoY < 0 ? 0 : videoY;
                    scrY = Convert.ToInt32(videoY * 1.6) - shiftY;
                    scrY = scrY > 0 ? scrY : 0;//最小x，y值为0,0
                    //GlobalLayers._skinfowindow.Width = 200;
                    //VideoToWall._closeallb20win();
                    if (_currentVideo.DB33!="")
                    {
                        VideoToWall._closeb20win(_currentVideo.winno, _currentVideo.ptr);
                    }
                    _currentVideo.winno = VideoToWall._openb20win(stdid, scrX, scrY, 1000, 1000, _currentVideo.ptr);//视频上墙
                    haveOpened = true;//已打开一个视频
                    GeomServControl.alVideosPlayed.Add(stdid);
                    //Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["playtime"])));


                       /*
                        string XH = feature.Attributes["XH"].ToString(); // 真实库为大写
                        if (!GlobalLayers._GraphicLogCollention.ContainsKey(XH)) // 没有打开过的
                        {
                            Graphic gp = null;

                            graphicsLayer = GlobalLayers._MainMap.Layers[LayerName] as GraphicsLayer;
                            gp = new Graphic();
                            gp.Geometry = feature.Geometry; //  new MapPoint(122.10324, 30.0219);
                            gp.Symbol = App.Current.Resources["ResultsSymbol"] as Symbol;

                            gp.Attributes["ID"] = XH;       // 视频编号为地图上的唯一ID
                                    // XH.Substring(12, 4);   // 取视频编号的倒数第二开始的后4位
                            // 在视频点名称后面加 4位编码，方便键盘控制
                            if (XH.Length == 18)
                                gp.Attributes["MC"] = feature.Attributes["MC"].ToString() + "(" + XH.Substring(12, 4) + ")";
                            else
                                gp.Attributes["MC"] = feature.Attributes["MC"].ToString();
                            if (LayerName == "DynamicResultGraphicLayer")
                                GlobalLayers.DynamicResultGraphicLayer.Graphics.Add(gp);
                            else
                                GlobalLayers.ResultGraphicLayer.Graphics.Add(gp);
                                                       
                            GlobalLayers._GraphicLogCollention.TryAdd(XH, gp); 
                        }                                 
                        String VID = feature.Attributes["XH"].ToString();
                        MapPoint Mp = feature.Geometry as MapPoint;
                        if (LayerName == "DynamicResultGraphicLayer") // 如果查询的是视频图层,在大屏上处理和GQY的通讯
                            GlobalLayers._gqyvideocontrol.FlushOpenVID(VID, Mp);
                        else
                            GlobalLayers._gqyvideocontrol.OpenVID(VID, Mp);
                        * */
// }));                                  
                    //}
        //   GlobalLayers.ResultGraphicLayer.Refresh();               
            }
    }
    catch (Exception e)
    {
//            MessageBox.Show(e.ToString());
    }
}

        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
      //           MessageBox.Show("Query failed: " + args.Error);
        }

    }
}
