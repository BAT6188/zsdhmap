using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Local;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Shapes;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Configuration;
using Newtonsoft.Json.Linq;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows;

namespace zsdpmap.classes
{
    class Traffic
    {
        FeatureLayer traffic;
        FeatureLayer traffic_lights;
        public Traffic()
        {

            try
            {
                traffic = GlobalLayers.TrafficLayer;
                traffic_lights = GlobalLayers.LightsLayer;

                traffic.OutFields = new OutFields { "*" };
                traffic_lights.OutFields = new OutFields { "*" };
            }
            catch(Exception)
            {

                return;
            }


            // 设定道路的流量渲染模型
            ClassBreaksRenderer rTrafficRoad = new ClassBreaksRenderer();   // 流量
            rTrafficRoad.Field = "SHAPE_Length";    // 指定渲染道路属性字段
            // 设定红绿灯的3状态渲染
            ClassBreakInfo _Normal = new ClassBreakInfo();
            _Normal.MinimumValue = 0;
            _Normal.MaximumValue = 100;
            _Normal.Symbol = App.Current.Resources["NORMAL"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            ClassBreakInfo _Middle = new ClassBreakInfo();
            _Middle.MinimumValue = 101;
            _Middle.MaximumValue = 200;
            _Middle.Symbol = App.Current.Resources["MIDDLE"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            ClassBreakInfo _High = new ClassBreakInfo();
            _High.MinimumValue = 201;
            _High.MaximumValue = 1000;
            _High.Symbol = App.Current.Resources["BUSY"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            rTrafficRoad.Classes.Add(_Normal);
            rTrafficRoad.Classes.Add(_Middle);
            rTrafficRoad.Classes.Add(_High);
            traffic.Renderer = rTrafficRoad;
            

            foreach (Graphic e in traffic as GraphicsLayer)
            {

                e.Attributes["SHAPE_Length"] = 0;
            }

            traffic.Visible = true;



            ClassBreaksRenderer rTrafficLights = new ClassBreaksRenderer();   // 红绿灯
            rTrafficLights.Field = "status";    // 指定渲染的红绿灯属性字段
            // 设定红绿灯的3状态渲染
            ClassBreakInfo r = new ClassBreakInfo();
            r.MinimumValue = 0;
            r.MaximumValue = 0;
            r.Symbol = App.Current.Resources["REDFLAG"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            ClassBreakInfo y = new ClassBreakInfo();
            y.MinimumValue = 1;
            y.MaximumValue = 1;
            y.Symbol = App.Current.Resources["YELLOWFLAG"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            ClassBreakInfo g = new ClassBreakInfo();
            g.MinimumValue = 2;
            g.MaximumValue = 2;
            g.Symbol = App.Current.Resources["GREENFLAG"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            rTrafficLights.Classes.Add(r);
            rTrafficLights.Classes.Add(y);
            rTrafficLights.Classes.Add(g);
            traffic_lights.Renderer = rTrafficLights;

            //         GlobalLayers._MainMap.Layers.Add(traffic_lights);
            foreach (Graphic e in traffic_lights as GraphicsLayer)
            {

                e.Attributes["status"] = 1;
            }

            traffic_lights.Visible = true;
            /*
            road_traffic_stat.Symbol = App.Current.Resources["TRAFFIC_NORMAL"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            traffic.Renderer = road_traffic_stat;
  */          

/*
            ArcGISLocalFeatureLayer FLAG = new ArcGISLocalFeatureLayer("dhnew\\traffic.mpk", "FLAG"); // 道路中心线_Clip/ FLAG
            SimpleRenderer traffic_stat = new SimpleRenderer();
            traffic_stat.Symbol = App.Current.Resources["REDFLAG"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            FLAG.Renderer = traffic_stat;
            GlobalLayers._MainMap.Layers.Add(FLAG);
            */


            try
            {
                IConnectionFactory factory;

                factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());   // 连接MQ数据发布服务器
                IConnection connection = factory.CreateConnection();
                connection.Start();
                ISession session = connection.CreateSession();
                // 连接红绿灯 主题
                IMessageConsumer consumerLights = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_Traffic_lights"].ToString()), "GQY_L", null, false);
                consumerLights.Listener += new MessageListener(ProcessLights);
                IMessageConsumer consumerTraffic = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_Traffic"].ToString()), "GQY_T", null, false);
                consumerTraffic.Listener += new MessageListener(ProcessTraffic);
            }
            catch(Exception e)
            {
                MessageBox.Show("订阅交通信号失败"+e.ToString());
            }
        }

        public void ProcessLights(IMessage message)
        {
            JObject rgstat;
            ITextMessage rgmsg_text = (ITextMessage)message;

            try
            {
                rgstat = JObject.Parse(rgmsg_text.Text);   // 从GPS数据量转换为内部对象
                string Traffic_lights_id = rgstat["FID"].ToString();
                int Traffic_lights_status = int.Parse(rgstat["STATUS"].ToString());

                    
                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                        delegate
                        {
                            foreach (Graphic e in traffic_lights)
                            {
                                if ((e.Attributes["FID"]).ToString() == Traffic_lights_id)
                                {
                                    e.Attributes["status"] = Traffic_lights_status;
                                    break;
                                }
                            }
                            traffic_lights.Refresh();
                            
                        }));


            }
            catch (Exception)
            {
                // 接收到异常信息直接抛弃
            }


        }

        public void ProcessTraffic(IMessage message)
        {
            JObject TrafficMsg;
            ITextMessage TrafficMsg_text = (ITextMessage)message;

            try
            {
                TrafficMsg = JObject.Parse(TrafficMsg_text.Text);   // 从GPS数据量转换为内部对象
                // 交通流量模型： 模拟器定时发送道路编号和流量数据，收到后更新指定道路编号的流量值，渲染器根据这个值改变颜色
                string ROADID = TrafficMsg["ID"].ToString();
                double value = double.Parse(TrafficMsg["VALUE"].ToString());

                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                        delegate
                        {
                            foreach (Graphic e in traffic)
                            {
                                if ((e.Attributes["OBJECTID"]).ToString() == ROADID)
                                {
                                    e.Attributes["SHAPE_Length"] = value;
                                    break;
                                }
                            }
                            traffic.Refresh();

                        }));


            }
            catch (Exception)
            {
                // 接收到异常信息直接抛弃
            }


        }
    }
}
