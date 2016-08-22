using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Local;
using System.Collections;
using System.Threading;
using ESRI.ArcGIS.Client.Symbols;
using System.Configuration;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace zsdpmap.classes
{
    /// <summary>
    /// LPY 
    /// </summary>
    public class GeomServControl
    {
        public GeometryService _geometryTask;
        public static ArrayList alPoints = new ArrayList();
        public static ArrayList alVideosPlayed = new ArrayList();    //已播放视频点集合
        public static ArrayList alVideoNeedToPlay = new ArrayList();//待播放视频点集合
        Thread threadPointsRun = null;
        public Thread threadVideoToWall = null;//视频点循环上墙的线程
        public static double searchRadius = Convert.ToDouble(ConfigurationManager.AppSettings["SearchRadius"]);
        public static bool flag = false;//标记是否开始做视频点循环上墙
        //public static ArrayList alVideos = new ArrayList();
        //public static ArrayList alVideosPlayed = new ArrayList();

        public GeomServControl()
        {
            _geometryTask = new GeometryService();
            _geometryTask.Url = LocalGeometryService.GetService().UrlGeometryService;
            _geometryTask.DensifyCompleted += _geometryTask_DensifyCompleted;
            _geometryTask.Failed += _geometryTask_Failed;
        }
        
        void _geometryTask_DensifyCompleted(object sender, GraphicsEventArgs e)
        {
            GraphicsLayer graphicsLayerLine = GlobalLayers._MainMap.Layers["graphicsLayerLine"] as GraphicsLayer;
            foreach (Graphic g in e.Results)
            {
                Polyline p = g.Geometry as Polyline;
                foreach (PointCollection pc in p.Paths)
                {
                    foreach (MapPoint point in pc)
                    {
                        Graphic newPoint = new Graphic() {
                            Symbol = App.Current.Resources["NewMarkerSymbol"] as Symbol,
                            Geometry=point
                        };
                        graphicsLayerLine.Graphics.Add(newPoint);

                        Graphic biggerNewPoint = new Graphic()
                        {
                            Symbol = App.Current.Resources["BiggerNewMarkerSymbol"] as Symbol,
                            Geometry = GeometryFunc.GetEllipseGraphic(searchRadius / (106 * 1000), point)//这里是米换算为经度，使用已存在的方法Mark
                        };
                        alPoints.Add(biggerNewPoint);
                    }
                }
            }
            threadPointsRun = new Thread(new ThreadStart(PointsRun));
            threadPointsRun.Start();
        }
        
        void _geometryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PointsRun()
        {
            while (true)//循环播放,遇到停止疏散演示的时候退出循环
            {
                Graphic graphicTemp = new Graphic();

                alVideosPlayed.Clear();//清空播放历史
                QueryLayer.lastPlayedid = "";
                QueryLayer._currentVideo.DB33 = "";

                if (alPoints.Count <= 0)//判断，是否突然停止疏散演示，停止演示会清空alPoints
                {
                    if (QueryLayer._currentVideo.DB33 != "")
                    {
                                   //GlobalLayers._skinfowindow.Width = 10;
                          VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                          QueryLayer.lastPlayedid = "";
                          QueryLayer.haveOpened = false;
                          QueryLayer._currentVideo.DB33 = "";
                          QueryLayer._currentVideo.winno = 0;

                    }
                    if (threadPointsRun != null)
                    {
                        if (threadPointsRun.IsAlive)
                        {
                            threadPointsRun.Abort();//Mark
                        }
                    }

                    break;
                }
                for (int i = 0; i < alPoints.Count; i++)        // 延加密度的点信息表上开始巡逻
                {
                    if (alPoints.Count <= 0)//判断，是否突然停止疏散演示，停止演示会清空alPoints
                    {
                        if (QueryLayer._currentVideo.DB33 != "")
                        {
                                //GlobalLayers._skinfowindow.Width = 10;
                                VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                                QueryLayer.lastPlayedid = "";
                                QueryLayer.haveOpened = false;
                                QueryLayer._currentVideo.DB33 = "";
                                QueryLayer._currentVideo.winno = 0;
                        }
                        if (threadPointsRun!=null)
                        {
                            if (threadPointsRun.IsAlive)
                            {
                                threadPointsRun.Abort();//Mark
                            }
                        }
                        
                        break;
                    }
                    graphicTemp = (Graphic)alPoints[i];
                    GlobalLayers._MainMap.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                try
                                {
                                    GraphicsLayer graphicsLayerLinePointsRun = GlobalLayers._MainMap.Layers["graphicsLayerLinePointsRun"] as GraphicsLayer;
                                    if (graphicsLayerLinePointsRun != null)
                                    {
                                        graphicsLayerLinePointsRun.Graphics.Clear();
                                        graphicsLayerLinePointsRun.Graphics.Add(graphicTemp);
                                        QueryLayer QL = new QueryLayer(graphicTemp.Geometry, "CJ_SP_PT", "DynamicResultGraphicLayer");
                                        QL.Do();
                                        Thread.Sleep(300);//等待QL.Do()完成再往下走                                               
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }));
                    
                    if (QueryLayer._currentVideo.DB33!="")
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(Convert.ToDouble(System.Configuration.ConfigurationManager.AppSettings["playtime"])));
                        //Thread.Sleep(100);
                        VideoToWall._closeb20win(QueryLayer._currentVideo.winno, QueryLayer._currentVideo.ptr);
                        QueryLayer.lastPlayedid = "";
                        QueryLayer._currentVideo.DB33 = "";
                    }
                    else
                        Thread.Sleep(100);      // 没有打开视频的时候
                    
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //这里要做循环播放视频点列表里的视频的动作
                //int ttt = alVideoNeedToPlay.Count;
                //int iii = 0;
                //if (!flag)
                //{
                //    UpdateSPLabel();
                //    flag = true;
                //}

            }
            //throw new NotImplementedException();
        }


        private void UpdateSPLabel()
        {
            int count = alVideoNeedToPlay.Count;
            int mode = 6;//6个视频底板模式
            int flag = 1;

            int playTime = count / mode;
            int playMore = count % mode;
            if (playMore>0)
            {
                playTime += 1;
            }
            
            

            GlobalLayers._MainMap.Dispatcher.Invoke(
                        new Action(
                            delegate
                            {
                                InitSPINFOPAD();

                                GlobalLayers._spinfowindow.Width = 1700;
                                GlobalLayers._spinfowindow.Height = 2160;
                                ThicknessAnimation taIn = new ThicknessAnimation();
                                taIn.From = new Thickness(1700, 0, 0, 0);
                                taIn.To = new Thickness(0, 0, 0, 0);
                                taIn.Duration = TimeSpan.FromSeconds(1);
                                GlobalLayers._spinfowindow.BeginAnimation(UserControl.BorderThicknessProperty, taIn);

                                CloseAllSP();

                                try
                                {

                                    while (true)
                                    {
                                        for (int i = 0; i < count; i++)
                                        {
                                            if (flag<=6)
                                            {
                                                flag++;
                                                int tempi = i % mode + 1;
                                                UpdateSPLabel(tempi.ToString(), tempi.ToString(), alVideoNeedToPlay[i].ToString());
                                            }
                                            else
                                            {
                                                flag = 1;
                                                Thread.Sleep(30);
                                                i--;//上移一位
                                            }
                                        }
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }));
            
        }


        //根据flagSPNum个数放置视频，改变视频背景名称
        private void UpdateSPLabel(string spName, string spid, string stdid)
        {
            int W = 1000;
            int H = 1000;
            //_b20controldata.ptr = Marshal.AllocHGlobal(128);
            if (GlobalLayers._spinfowindow.tb1.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb1.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb1.Tag = stdid;

                CMDOperator._b20controldata[0].DB33 = stdid;
                //CMDOperator._b20controldata[0].winno = VideoToWall._openb20win(stdid, 5665, 280, W, H, CMDOperator._b20controldata[0].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb2.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb2.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb2.Tag = stdid;
                CMDOperator._b20controldata[1].DB33 = stdid;
                //CMDOperator._b20controldata[1].winno = VideoToWall._openb20win(stdid, 6680, 280, W, H, CMDOperator._b20controldata[1].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb3.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb3.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb3.Tag = stdid;
                CMDOperator._b20controldata[2].DB33 = stdid;
                //CMDOperator._b20controldata[2].winno = VideoToWall._openb20win(stdid, 5665, 1380, W, H, CMDOperator._b20controldata[2].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb4.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb4.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb4.Tag = stdid;
                CMDOperator._b20controldata[3].DB33 = stdid;
                //CMDOperator._b20controldata[3].winno = VideoToWall._openb20win(stdid, 6680, 1380, W, H, CMDOperator._b20controldata[3].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb5.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb5.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb5.Tag = stdid;
                CMDOperator._b20controldata[4].DB33 = stdid;
                //CMDOperator._b20controldata[4].winno = VideoToWall._openb20win(stdid, 5665, 2480, W, H, CMDOperator._b20controldata[4].ptr);
            }
            else if (GlobalLayers._spinfowindow.tb6.Tag.ToString() == "0")
            {
                GlobalLayers._spinfowindow.tb6.Text = spid + "-" + spName;
                GlobalLayers._spinfowindow.tb6.Tag = stdid;
                CMDOperator._b20controldata[5].DB33 = stdid;
                //CMDOperator._b20controldata[5].winno = VideoToWall._openb20win(stdid, 6680, 2480, W, H, CMDOperator._b20controldata[5].ptr);
            }
            else
            {
                //多于6个，过多，抛弃
            }
        }

        //视频下窗的时候，背景板改变
        private void UpdateSPLabelOff(string spid, string stdid)
        {
            if (GlobalLayers._spinfowindow.tb6.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb6.Text = "";
                GlobalLayers._spinfowindow.tb6.Tag = "0";
                if (CMDOperator._b20controldata[5].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[5].winno, CMDOperator._b20controldata[5].ptr);
                    CMDOperator._b20controldata[5].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb5.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb5.Text = "";
                GlobalLayers._spinfowindow.tb5.Tag = "0";
                if (CMDOperator._b20controldata[4].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[4].winno, CMDOperator._b20controldata[4].ptr);
                    CMDOperator._b20controldata[4].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb4.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb4.Text = "";
                GlobalLayers._spinfowindow.tb4.Tag = "0";
                if (CMDOperator._b20controldata[3].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[3].winno, CMDOperator._b20controldata[3].ptr);
                    CMDOperator._b20controldata[3].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb3.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb3.Text = "";
                GlobalLayers._spinfowindow.tb3.Tag = "0";
                if (CMDOperator._b20controldata[2].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[2].winno, CMDOperator._b20controldata[2].ptr);
                    CMDOperator._b20controldata[2].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb2.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb2.Text = "";
                GlobalLayers._spinfowindow.tb2.Tag = "0";
                if (CMDOperator._b20controldata[1].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[1].winno, CMDOperator._b20controldata[1].ptr);
                    CMDOperator._b20controldata[1].DB33 = "";
                }
            }
            else if (GlobalLayers._spinfowindow.tb1.Tag.ToString() == stdid)
            {
                GlobalLayers._spinfowindow.tb1.Text = "";
                GlobalLayers._spinfowindow.tb1.Tag = "0";
                if (CMDOperator._b20controldata[0].DB33 != "")
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[0].winno, CMDOperator._b20controldata[0].ptr);
                    CMDOperator._b20controldata[0].DB33 = "";
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
        }

        //关闭所有已打开的视频
        private void CloseAllSP()
        {
            for (int i = 0; i < CMDOperator._b20controldata.Length; i++)
            {
                if (CMDOperator._b20controldata[i].DB33 != "")//存在视频，关闭
                {
                    VideoToWall._closeb20win(CMDOperator._b20controldata[i].winno, CMDOperator._b20controldata[i].ptr);
                }
            }
            //VideoToWall._closeallb20win();
        }
        
    }
}
