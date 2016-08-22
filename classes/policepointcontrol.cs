using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows;
using System.Threading;

namespace zsdpmap.classes
{
    class ThreadParam
    {
        /*
        public JObject jsoncommand;
        public GraphicsLayer policeGlyr;
        public GraphicsLayer linklineGlyr;
        public GraphicsLayer crimeGlyr;
        public Map mainmap;
         */
       // public Hashtable _videocollection;
        public ConcurrentDictionary<string,Object> _videocollection = new ConcurrentDictionary<string,object>();
        /*
        public ThreadParam(JObject _jsoncommand, GraphicsLayer _policeGlyr, GraphicsLayer _linklineGlyr, GraphicsLayer _crimeGlyr, Map _mainmap, Hashtable __videocollection)
        {
            jsoncommand = _jsoncommand;
            policeGlyr = _policeGlyr;
            linklineGlyr = _linklineGlyr;
            crimeGlyr = _crimeGlyr;
            mainmap = _mainmap;
            _videocollection = __videocollection;
                 
        }
         */
    }

    public class policepointcontrol
    {
        private crimepointcontrols _crimepointcontrols = new crimepointcontrols();
        private linklinecontrol _linklinecontrol = new linklinecontrol();
        private gqyvideocontrol _gqyvideocontrol = new gqyvideocontrol();
        string commandstr = "";

        

        bool GQYVOPEN(String VID, double pointx, double pointy, Map mainmap)
        {
            double OffsetX = 60;
            double OffsetY = 60;
            /*
            if (GlobalLayers._gqyvideocontrol._videocollection.ContainsKey(VID))     // 更新视频ID及其位置，用于地图缩放平移后向GQY刷新位置
                GlobalLayers._gqyvideocontrol._videocollection.Remove(VID);          // add by john 因为现在open已经改为可重复打开
            GlobalLayers._gqyvideocontrol._videocollection.Add(VID, new MapPoint(pointx, pointy)); //add by lzd
             * */

            try
            {
                commandstr = "OpenWin,";
                MapPoint M = new MapPoint(pointx, pointy);
                M.SpatialReference = new SpatialReference(4326);
                Point winpoint = mainmap.MapToScreen(M);
                commandstr = commandstr + VID + ",";
                commandstr = commandstr + (winpoint.X+OffsetX).ToString() + "," + (winpoint.Y+OffsetY).ToString() + ",";
                commandstr = commandstr + "800,600" + "#";  // 约定以 # 为指令码的结束符
                if (_gqyvideocontrol.linkgqy() != null)
                {
                    _gqyvideocontrol.sendcommandTCP(commandstr);
                    _gqyvideocontrol.closegqy();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 在地图上绘制动态点
        public bool addpolicecarpoint(JObject jsoncommand)
        {
            Symbol SelectedSymbol = null;
            Graphic SelectedGraphic = null;
            Graphic CrimeGraphic = null;
            bool HasNew = false;
            string HH = null ;

            try
            {
                string policetype = jsoncommand["TYPE"].ToString().Trim();

                if (policetype == "30") // 案件点
                {
                    double pointx = (double)jsoncommand["X"];
                    double pointy = (double)jsoncommand["Y"];
                    
                    string title = jsoncommand["Title"].ToString();   // 案件点用编号为ID，（ 已经改为用HH 为唯一ID）
                    HH = jsoncommand["HH"].ToString();   // 案件点用呼号为ID
                    CrimeGraphic = mapcontrol.getMapG(HH, GlobalLayers._crimepointglr);
                    
                    if (CrimeGraphic == null)
                    {
                        CrimeGraphic = new Graphic();
                        CrimeGraphic.Attributes["ID"] = HH;
                        CrimeGraphic.Attributes["num"] = title;
                        CrimeGraphic.Geometry = new MapPoint(pointx, pointy);
                        CrimeGraphic.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
                        GlobalLayers._crimepointglr.Graphics.Add(CrimeGraphic);

                    }
                    else  // 如果已经有信息在图层里，只更新属性
                    {
                        CrimeGraphic.Geometry = new MapPoint(pointx, pointy);
                        CrimeGraphic.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
                    }

                }
                else
                {
                    double pointx = (double)jsoncommand["X"];
                    double pointy = (double)jsoncommand["Y"];
                    double angle = (double)jsoncommand["DIRECT"];
                    double speed = (double)jsoncommand["SPEED"];
                    string video = jsoncommand["VIDEOONOFF"].ToString().Trim();
                    string VID = jsoncommand["VID"].ToString().Trim();
                    string status = jsoncommand["Status"].ToString().Trim();
                    double linkx = (double)jsoncommand["linktox"];
                    double linky = (double)jsoncommand["linktoy"];


                    HH = jsoncommand["HH"].ToString(); // GPS 唯一编号
                    string title = jsoncommand["Title"].ToString();

                    Graphic pgr = mapcontrol.getMapG(HH, GlobalLayers._policepointglr);     // 检查有无该车在地图上
                    Graphic OffCar = mapcontrol.getMapG(HH, GlobalLayers._offlineglr);  // 检查是否在离线层上
                    Graphic pgrline = mapcontrol.getMapG(HH, GlobalLayers._linklineglr);

                    if (pgr == null && OffCar == null) // 在线和离线图层都没有该车
                    {
                        HasNew = true;
                        SelectedGraphic = new Graphic();

                    }
                    else
                    {
                        if (pgr != null)                    // 选择在线车或者离线车的对应图层
                            SelectedGraphic = pgr;
                        else
                            SelectedGraphic = OffCar;
                    }

                    SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                    SelectedGraphic.Attributes["ID"] = HH;
                    SelectedGraphic.Attributes["num"] = title;

                    // 如果 状态为离线， 只在 19，20 级显示，设计离线图层

                    switch (policetype)
                    {
                        //车辆
                        case "04":
                       //     if (HasNew || speed != 0)         // 如果收到的点速度为0 ，不变更上一点的方向
                             SelectedGraphic.Attributes["angle"] = angle;
                             //根据状态设置服务
                            // 
                           //  错误，状态改变时 不会改变图标  if (HasNew) // 只有新的车才装载符号   
                           // {

                                switch (status) 
                                {
                                    case "0":
                                        SelectedSymbol = App.Current.Resources["policecar0"] as Symbol;
                                        break;
                                    case "1":
                                        SelectedSymbol = App.Current.Resources["policecar1"] as Symbol;
                                        break;
                                    case "2":       // 离线车辆处理

                                        SelectedSymbol = App.Current.Resources["policecar2"] as Symbol;
                                        break;
                                }
                                SelectedGraphic.Symbol = SelectedSymbol;

                                
                            //}



                            break;

                        //摩托车
                        case "14":
                            SelectedGraphic.Symbol = App.Current.Resources["policemoto0"] as Symbol;
                            /*
                            if (HasNew || speed != 0)         // 如果收到的点速度为0 ，不变更上一点的方向
                                pgr.Attributes["angle"] = angle;
                            //根据状态设置服务
                            switch (status) 
                            {
                                case "0":
                                    pgr.Symbol = App.Current.Resources["policemoto0"] as Symbol;
                                    break;
                                case "1":
                                    pgr.Symbol = App.Current.Resources["policecar1"] as Symbol;
                                    break;
                                case "2":
                                    pgr.Symbol = App.Current.Resources["policecar2"] as Symbol;
                                    break;
                                default:
                                    break;
                            } 
                             */
                            break;

                        //社会车辆
                        case "20":
                            if (HasNew || speed != 0)         // 如果收到的点速度为0 ，不变更上一点的方向
                                pgr.Attributes["angle"] = angle;
                            //根据状态设置服务
                            
                            switch (status) 
                            {
                                case "0":
                                    SelectedSymbol = App.Current.Resources["policecar0"] as Symbol;
                                    break;
                                case "1":
                                    SelectedSymbol = App.Current.Resources["policecar1"] as Symbol;
                                    break;
                                case "2":
                                    SelectedSymbol = App.Current.Resources["policecar2"] as Symbol;
                                    break;
                            }
                            SelectedGraphic.Symbol = SelectedSymbol;
                            
                            break;

                        //人
                        case "09":  // 人图标没有旋转
                                SelectedGraphic.Symbol = App.Current.Resources["policeman0"] as Symbol;
                            /*
                            switch (status) 
                            {
                                case "0":
                                    pgr.Symbol = App.Current.Resources["policeman0"] as Symbol;
                                    break;
                                case "1":
                                    pgr.Symbol = App.Current.Resources["policecar1"] as Symbol;
                                    break;
                                case "2":
                                    pgr.Symbol = App.Current.Resources["policecar2"] as Symbol;
                                    break;
                                default:
                                    break;
                            } 
                             */
                            break;
                    }

                    if (HasNew)
                    {
                        if (status != "2")
                            GlobalLayers._policepointglr.Graphics.Add(SelectedGraphic);
                        else
                            GlobalLayers._offlineglr.Graphics.Add(SelectedGraphic);
                    }
                    else
                    {
                        if (pgr != null && status == "2")   // 原来状态在线 ,现在离线
                        {
                            GlobalLayers._policepointglr.Graphics.Remove(pgr);  // 从在线层移出
                            GlobalLayers._offlineglr.Graphics.Add(pgr);
                        }
                        if (OffCar != null && status != "2")    // 原来离线，现在不离线
                        {
                            GlobalLayers._offlineglr.Graphics.Remove(OffCar);  // 从在线层移出
                            GlobalLayers._policepointglr.Graphics.Add(OffCar);
                        }
                    }

                    if (linkx != 0.0)                    //如果车信息有连接目标，添加连接线
                    {
                        if (pgrline == null)
                            _linklinecontrol.Addlinkline(new MapPoint(pointx, pointy), new MapPoint(linkx, linky),GlobalLayers._linklineglr, HH);
                        else
                        {
                            GlobalLayers._linklineglr.Graphics.Remove(pgrline);
                            _linklinecontrol.Addlinkline(new MapPoint(pointx, pointy), new MapPoint(linkx, linky), GlobalLayers._linklineglr, HH);  //add by lzd
                        }
                    }
                    else
                    {
                        if (pgrline != null)
                        {
                            GlobalLayers._linklineglr.Graphics.Remove(pgrline);
                        }
                    }
                    //打开视频
                    if (video == "1" || GlobalLayers._gqyvideocontrol._videocollection.ContainsKey(VID))  // 如果车载视频被控制端驱动打开，会在VID 容器里保留。 
                        GQYVOPEN(VID, pointx, pointy, GlobalLayers._MainMap);

                }
                
                return true;
            }
            catch (Exception)
            {
          //      MessageBox.Show(e.ToString());
        //        MessageBox.Show(jsoncommand.ToString());
                return false;
            }

        }
    }

        #endregion

}
