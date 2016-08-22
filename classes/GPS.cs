using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using System.Configuration;
using System.Windows;



namespace zsdpmap.classes
{

    // 从MQ 获取GPS信息
    class _GPS
    {
      //  static Graphic ActivePoint = null;
    //    static GraphicsLayer ActiveLayer ; //= new GraphicsLayer();

        public _GPS()
        {
            try
            {
                IConnectionFactory factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());   // 连接MQ数据发布服务器
                IConnection connection = factory.CreateConnection();
                connection.Start();
                ISession session = connection.CreateSession();
                // 连接GPS 主题
             //   IMessageConsumer consumer = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_GPS"].ToString()), "GQY_GPS", null, false);
                IMessageConsumer consumer = session.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_GPS"].ToString()));
              
                consumer.Listener += new MessageListener(ProcessGPS);
                
            }
            catch(Exception e)
            {
                MessageBox.Show("订阅GPS主题失败" + e.Message);
            }
        }

        public void ProcessGPS(IMessage message)
        {
            JObject gpsobj;
            ITextMessage gpsmsg_text = (ITextMessage)message;

            try
            {
                gpsobj = JObject.Parse(gpsmsg_text.Text);   // 从GPS数据量转换为内部对象

                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                        delegate
                        {

                            DrawGPS(gpsobj);
                        }));


            }
            catch (Exception)
            {
                // 接收到异常信息直接抛弃
            }


        }

        #region 在地图上绘制动态点
        public void addpolicecarpoint(JObject GpsData)
        {
            TimeSpan OFFLINETIME = new TimeSpan(0, 0, 15, 0);    // 判定为离线的时间间隔

            Symbol SelectedSymbol = null;
            Graphic SelectedGraphic = null;

            bool HasNew = false;
            string HH = null ;

            //if (CarFilter.ModifyFilterLock) // 如果正在重置过滤器，不绘制
            //    return;

            try
            {
                HH = GpsData["HH"].ToString(); // GPS 唯一编号
                //if (!CarFilter.FindKey(HH)) // 如果该车不在过滤表内，不处理
                //    return;
                string policetype = GpsData["TYPE"].ToString().Trim();
                if (policetype == "0")
                    policetype = "3";
                double pointx = (double)GpsData["X"];
                double pointy = (double)GpsData["Y"];
                double angle = (double)GpsData["DIRECT"];
                double speed = (double)GpsData["SPEED"];
                string status;
                try
                {
                    status = GpsData["STATUS"].ToString().Trim();
                }
                catch(Exception)
                {
                    status = "2";   // 普通，绿色
                }


                /* 车辆的动态点因为频率较高，在大屏上影响显示效率
                if (ActivePoint == null)
                {
                    ActivePoint = new Graphic();
                    ActivePoint.Symbol = App.Current.Resources["NEWGPS"] as Symbol;
                    ActiveLayer.Graphics.Add(ActivePoint);
                }

                ActivePoint.Geometry = new MapPoint(pointx, pointy);
                */

                string title = GpsData["TITLE"].ToString();


                DateTime RKSJ;
                try
                {
                    RKSJ = (DateTime)GpsData["RKSJ"];
                }
                catch
                {
                    RKSJ = DateTime.Now;
                }
                Graphic OffCar = mapcontrol.getMapG(HH, GlobalLayers._offlineglr);  // 检查是否在离线层上   
                Graphic pgr = mapcontrol.getMapG(HH, GlobalLayers._policepointglr);     // 检查有无该车在地图上
                TimeSpan ts = DateTime.Now - RKSJ;

                if (ts >= OFFLINETIME) // 信息点超时，判断为离线
                {

                    if (OffCar != null) // 已经在离线图层，不做任何处理返回                  
                        return;
                    else
                    {
                        if (pgr != null) // 这个点原来在在线图层
                        {
                            // 先把这个车从在线图层中移走，再放到离线图层
                            GlobalLayers._policepointglr.Graphics.Remove(pgr);
                            GlobalLayers._offlineglr.Graphics.Add(pgr);
                            SelectedGraphic = pgr;
                            SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                            SelectedGraphic.Attributes["ID"] = HH;
                            SelectedGraphic.Attributes["num"] = title;
                            SelectedGraphic.Attributes["RKSJ"] = RKSJ.ToString();
                            status = "0"; // 离线符号
                        }
                        else  // 离线在线图层都没有
                        {
                            // 插入离线图层
                            SelectedGraphic = new Graphic();
                            SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                            SelectedGraphic.Attributes["ID"] = HH;
                            SelectedGraphic.Attributes["num"] = title;
                            SelectedGraphic.Attributes["RKSJ"] = RKSJ.ToString();
                            GlobalLayers._offlineglr.Graphics.Add(SelectedGraphic);
                            status = "0";
                        }
                    }

                }
                else  // 信息点当前非离线
                {
                    if (OffCar != null) // 上次在离线图层
                    {
                        // 从离线图层中移走
                        // 插入到在线图层
                        GlobalLayers._offlineglr.Graphics.Remove(OffCar);
                        GlobalLayers._policepointglr.Graphics.Add(OffCar);
                        SelectedGraphic = OffCar;
                        SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                        SelectedGraphic.Attributes["ID"] = HH;
                        SelectedGraphic.Attributes["num"] = title;
                        SelectedGraphic.Attributes["RKSJ"] = RKSJ.ToString();
                    }
                    else
                    {
                        if (pgr != null)
                        {
                            // 判断是否要修改点
                            if (pgr.Attributes["RKSJ"].ToString() == RKSJ.ToString()) // 入库时间没有变，不处理
                                return;
                            else
                            {
                                SelectedGraphic = pgr;
                                SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                                SelectedGraphic.Attributes["ID"] = HH;
                                SelectedGraphic.Attributes["num"] = title;
                                SelectedGraphic.Attributes["RKSJ"] = RKSJ.ToString();
                            }
                        }
                        else
                        {
                            // 插入新点
                            SelectedGraphic = new Graphic();
                            SelectedGraphic.Geometry = new MapPoint(pointx, pointy);
                            SelectedGraphic.Attributes["ID"] = HH;
                            SelectedGraphic.Attributes["num"] = title;
                            SelectedGraphic.Attributes["RKSJ"] = RKSJ.ToString();
                            GlobalLayers._policepointglr.Graphics.Add(SelectedGraphic);
                        }
                    }

                }

                    switch (policetype)
                    {
                        //车辆
                        case "1":
                            //     if (HasNew || speed != 0)         // 如果收到的点速度为0 ，不变更上一点的方向
                            SelectedGraphic.Attributes["angle"] = angle;
                            //根据状态设置服务
                            // 
                            //  错误，状态改变时 不会改变图标  if (HasNew) // 只有新的车才装载符号   
                            // {

                            switch (status)
                            {
                                case "2":
                                    SelectedSymbol = App.Current.Resources["policecar0"] as Symbol;
                                    break;
                                case "3":
                                    SelectedSymbol = App.Current.Resources["policecar1"] as Symbol;
                                    break;
                                case "1":       // 离线车辆处理

                                    SelectedSymbol = App.Current.Resources["policecar1"] as Symbol;
                                    break;
                                case "0":
                                    SelectedSymbol = App.Current.Resources["policecar2"] as Symbol;
                                    break;
                            }
                            SelectedGraphic.Symbol = SelectedSymbol;


                            //}



                            break;

                        //摩托车
                        case "2":
                            SelectedGraphic.Symbol = App.Current.Resources["policemoto0"] as Symbol;

                            break;

                        //电瓶车
                        case "99":
                            if (HasNew || speed != 0)         // 如果收到的点速度为0 ，不变更上一点的方向
                                pgr.Attributes["angle"] = angle;
                            //根据状态设置服务

                            switch (status)
                            {
                                case "2":
                                    SelectedSymbol = App.Current.Resources["policeelecbike0"] as Symbol;//在线
                                    break;
                                case "3":
                                    SelectedSymbol = App.Current.Resources["policeelecbike0"] as Symbol;
                                    break;
                                case "1":
                                    SelectedSymbol = App.Current.Resources["policeelecbike0"] as Symbol;
                                    break;
                                case "0":
                                    SelectedSymbol = App.Current.Resources["policeelecbike2"] as Symbol;//离线
                                    break;
                            }
                            SelectedGraphic.Symbol = SelectedSymbol;

                            break;

                        //人
                        case "88":  // 人图标没有旋转
                            switch (status)
	                        {
                                case "0"://离线
                                    SelectedGraphic.Symbol = App.Current.Resources["policeman1"] as Symbol;
                                    break;
                                case "2"://在线
                                    SelectedGraphic.Symbol = App.Current.Resources["policeman0"] as Symbol;
                                    break;
                                default:
                                    SelectedGraphic.Symbol = App.Current.Resources["policeman1"] as Symbol;
                                    break;
	                        }
                            
                            break;
                        default:
                            break;
                    }

                
            } catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
            }


        }


        #endregion

        void DrawGPS(JObject gps)
        {

  //          if (gps["CMD"].ToString() != "00080") // 以后这里可以放验证码
  //              return;
            //if (!CarFilter.FindKey(gps["HH"].ToString()))  // 如果该车不在过滤器允许范围内，不做任何处理
            //    return;

            addpolicecarpoint(gps);

            if (gps.Property("HH") != null)  // 简单处理， 有GPS 呼号的点才处理刷新
            {
                string Key = gps["HH"].ToString();   // GPS ID 

                if (PublicVARS.MainGPS == gps["HH"].ToString())
                {
                    PublicVARS.MainX = (double)gps["X"];
                    PublicVARS.MainY = (double)gps["Y"];
                    PublicVARS.Speed = (double)gps["SPEED"];
                    PublicVARS.Direct = (double)gps["DIRECT"];
                }
            }

        }
    }
}
