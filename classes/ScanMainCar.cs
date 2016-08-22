using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Symbols;
using System.Collections;


namespace zsdpmap.classes
{
    class ScanMainCar
    {



        private void DoLine()
        {
              QueryWithBuffer QWB;
              QWB = new QueryWithBuffer("DynamicGraphicLayer", PublicVARS.Distance);
            double _DIRECT;
            if (PublicVARS.MainGPS != null && PublicVARS.MainX != 0.0)
            {
                _DIRECT = 360 + 90 - PublicVARS.Direct; // 转为逆时针，0 度 为

                    try
                    {
                        if (PublicVARS.Speed <= 0)
                            return;
                        Polyline PL = new Polyline();
                        double degree = PublicVARS.Distance / (106 * 1000);   // 距离米 换算成度
                 //      GlobalLayers.MainX += 0.0005;    // 模拟移动
                 //       GlobalLayers.MainY -= 0.0005;


                        double NextX;
                        double NextY;
                        NextX = PublicVARS.MainX + degree * Math.Cos(_DIRECT);
                        NextY = PublicVARS.MainY - degree * Math.Sin(_DIRECT);

                        ESRI.ArcGIS.Client.Geometry.PointCollection points = new ESRI.ArcGIS.Client.Geometry.PointCollection();
                        points.Add(new MapPoint(PublicVARS.MainX, PublicVARS.MainY));
                        points.Add(new MapPoint(NextX, NextY));
                        PL.Paths.Add(points);   // 制作一个条状的缓冲区


                        Geometry g = PL;    // 用一个团圆区做搜索视频范围

                        QWB.SetBuffGeom(g);

                        GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                         new Action(
                              delegate
                              {
                                  GlobalLayers.DynamicGraphicLayer.ClearGraphics();
                                  Graphic Car = new Graphic();
                                  Car.Geometry = new MapPoint(PublicVARS.MainX, PublicVARS.MainY);
                                  Car.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
                                  GlobalLayers.DynamicGraphicLayer.Graphics.Add(Car);
                              //    GlobalLayers._gqyvideocontrol.OpenCARVID(true, Car.Geometry as MapPoint);
                                  GlobalLayers._gqyvideocontrol.OpenVID(PublicVARS.MainVID, Car.Geometry as MapPoint);
                              }
                         ));
                        QWB.ProcessBuffer();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }

        }

  
        // 车辆动态跟踪查询
        // 车辆的位置点由接收GPS位置线程更新到GLOBALLAYERS.MAINX,MAINY
        // 每次循环查询车辆位置周边的视频，并发送打开指令到GQY的视频端口
        // 打开的视频如果不在车辆覆盖范围，到定时时间自动关闭

        public void AsyncLoop()
        {

            Graphic Effect = null;
         //   GlobalLayers.DynamicGraphicLayer.ClearGraphics();      // 动态特效的位置点
            while (true)
            {

                if (PublicVARS.MainGPS != null && PublicVARS.MainX != 0.0)      // 有跟踪点
                {
                    double Radius = PublicVARS.Distance / (106 * 1000);   // 米换算为经纬度

                    if (PublicVARS.TESTMODE)  // 测试模式时，X 自动加
                        PublicVARS.MainX += 0.00005;

                    Geometry SearchG = GeometryFunc.GetEllipseGraphic(Radius, new MapPoint(PublicVARS.MainX, PublicVARS.MainY));

                    GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                    delegate
                    {
                        Graphic TrackPoint = new Graphic();                    // 轨迹点
                        TrackPoint.Geometry = new MapPoint(PublicVARS.MainX, PublicVARS.MainY);
                        TrackPoint.Symbol = App.Current.Resources["CarPositionSymbol"] as Symbol;  // 红点轨迹点
                        GlobalLayers.DynamicResultGraphicLayer.Graphics.Add(TrackPoint);
  

                        if (Effect == null)
                        {
                            Effect = new Graphic();                        // 特效位置点
                            Effect.Symbol = App.Current.Resources["NEWGPS"] as Symbol;    // 设定特效点符号
                            Effect.Attributes["ID"] = "EFFECT";
                        }
                        Effect.Geometry = new MapPoint(PublicVARS.MainX, PublicVARS.MainY); // 更新特效点位置
                        if (mapcontrol.getMapG("EFFECT", GlobalLayers.DynamicGraphicLayer)==null)
                            GlobalLayers.DynamicGraphicLayer.Graphics.Add(Effect);
                        

                        // 显示查询覆盖区域

                        Graphic finder = null;
                        foreach (Graphic CB in GlobalLayers.DynamicGraphicLayer)    // 查找有没上一次显示的区域，没有做插入，没有做更新
                        {
                            if (CB.Attributes.ContainsKey("ID"))
                                if (CB.Attributes["ID"].ToString() == "AREA")
                                {
                                    finder = CB;
                                    break;

                                }
                        }
                        if (finder != null)
                            finder.Geometry = SearchG;
                        else
                        {


                            Graphic SearchArea = new Graphic();
                            SearchArea.Geometry = SearchG;
                            SearchArea.Attributes["ID"] = "AREA";

                            SearchArea.Symbol = App.Current.Resources["CarBufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                            GlobalLayers.DynamicGraphicLayer.Graphics.Add(SearchArea);
                        }

                    }
                    ));
                    // 做周边查询

                    GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                    delegate
                    {
                        //QueryLayer QL = new QueryLayer(SearchG, "CJ_SP_PT", "DynamicResultGraphicLayer");
                        //QL.Do();
                        //        QWB.SetBuffGeom(new MapPoint(GlobalLayers.MainX, GlobalLayers.MainY));  // 设定新的查询点
                        //        QWB.ProcessBuffer();

                        if (PublicVARS.MainVID != null && PublicVARS.MainVID != "")    // 车载视频
                            GlobalLayers._gqyvideocontrol.OpenVID(PublicVARS.MainVID, new MapPoint(PublicVARS.MainX, PublicVARS.MainY));


                        GlobalLayers._gqyvideocontrol.FlushEnd();   // 检查是否有到设定时间的视频，并关闭之
                    }
                    ));
                }
                Thread.Sleep(3000);
            }
        }
    }
}
