using Apache.NMS;
using Apache.NMS.ActiveMQ;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using zsdpmap.classes;

namespace zsdpmap
{
    /// <summary>
    /// AJ_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class AJ_INFOPAD_SMALL: UserControl
    {
        class DataSources
        {
            static int MAXQUEUE = 5;
            public static LinkedList<Information> AJQUEUE = new LinkedList<Information>();
            static int count = 0;
            public static DataGrid GlobalAJ = null;
            // 循环
            public static void LoopQueue(Information info)
            {
                if (count == MAXQUEUE)
                    AJQUEUE.RemoveLast();
                else
                    count++;
                AJQUEUE.AddFirst(info);
            }
        }
        class Information
        {
            string _AJBH;
            string _BJSJ;
            string _BJDH;
            string _BJR;
            string _AFDZ;
            string _BJNR;
            string _BJLXMC;

            public Information(JObject obj)
            {
                try
                {
                    this._AJBH = obj["JJDBH"].ToString();
                    this._BJSJ = (obj["BJSJ"].ToString()).Substring(0,19);
                    this._BJDH = obj["BJDH"].ToString();
                    this._BJR = obj["BJRXM"].ToString();
                    this._AFDZ = obj["SFDZ"].ToString();
                    this._BJNR = obj["BJNR"].ToString();
                    this._BJLXMC = obj["BJLXMC"].ToString();
                }
                catch(Exception)
                {

                }
            }

            public string AJBH
            {
                get
                { return _AJBH; }

            }
            public string BJSJ
            {
                get
                {
                    return _BJSJ;
                }
            }
            public string BJDH
            {
                get
                { return _BJDH; }

            }
            public string BJR
            {
                get
                {
                    return _BJR;
                }
            }
            public string AFDZ
            {
                get
                { 
                    return _AFDZ; 
                }

            }
            public string BJNR
            {
                get
                {
                    return _BJNR;
                }
            }
            public string BJLXMC
            {
                get
                {
                    return _BJLXMC;
                }
            }
        }


        IMessageConsumer consumer;
        ISession session;
        

        static Graphic ActivePoint = null;
        Information aj = null;

        static Dictionary<int, double> AJTJ = new Dictionary<int, double>();

        public AJ_INFOPAD_SMALL()
        {
            InitializeComponent();
         //   RefreshAJ();
            Subscribe();

        }
        
        void ReCalc()
        {
            AJTJ.Clear();
            for (int i = 0; i < 24; i++)
                AJTJ.Add(i, 0);

            foreach (Graphic e in GlobalLayers._crimepointglr)
            {
                DateTime bjsj = DateTime.Parse(e.Attributes["BJSJ"].ToString());

             //   double lastcount;
            //    AJTJ.TryGetValue(bjsj.Hour, out lastcount);
                AJTJ[bjsj.Hour] += 1.0; // 如果在这个小时有案件，加1

            }
        }
        void RefreshAJ()
        {
            while (true)
            {
                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                     new Action(
                         delegate
                         {

                             if (GlobalLayers._crimepointglr != null)
                             {
//                                 GlobalLayers._crimepointglr.Refresh();
//
 //                                GlobalLayers._crimepointglr.Visible = true;
                                 ReCalc();
                             }
                         }));
                Thread.Sleep(5000);
            }
            }
        


        public void Subscribe()
        {
            try
            {

                try
                {
                    IConnectionFactory factory;

                    factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());   // 连接MQ数据发布服务器
                    IConnection connection = factory.CreateConnection();
                    connection.Start();
                    session = connection.CreateSession();
                    consumer = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_AJ"].ToString()), "GQY_AJ", null, false);

                    AJCOUNT.ItemsSource = AJTJ;

                    consumer.Listener += new MessageListener(consumer_Listener);
                }
                catch(Exception e)
                {
                    MessageBox.Show("订阅案件主题失败"+e.Message);
                }

                //  Thread AJProcessThread = new Thread(new ThreadStart(ProcessAJ));
                //  AJProcessThread.Start();
                Task RecalcAJ = new Task(RefreshAJ);
                RecalcAJ.Start();
                
            }
            catch (Exception)
            {

            }
        }

        void consumer_Listener(IMessage message)
        {
            try
            {

                ITextMessage msg = (ITextMessage)message;
                JObject obj;

                obj = JObject.Parse(msg.Text);


                aj = new Information(obj);
       //         if (aj == null)
          //          return;

            //    RefreshAJ();        // 更新案件图层，并刷新统计值
                AJLST.Dispatcher.Invoke(
                    new Action(
                        delegate
                        {
                            DataSources.LoopQueue(aj);
                            
                            AJLST.ItemsSource = null;
                            AJLST.ItemsSource = DataSources.AJQUEUE;
                            JJDBH.Text = aj.AJBH;
                            AFDZ.Text = aj.AFDZ;
                            BJLB.Text = aj.BJLXMC;
                            BJNR.Text = aj.BJNR;
                            BJDH.Text = aj.BJDH;
                            BJSJ.Text = aj.BJSJ;
                            AJCOUNT.ItemsSource = null;
                            AJCOUNT.ItemsSource = AJTJ;
                        }));
                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                        delegate
                        {
                            DrawAJ(obj);  // 地图上绘制动态点
                        }));


  

          //      Console.WriteLine("Receive: " + msg.Text);
            }
            catch (System.Exception e)
            {
         //       Console.WriteLine(e.Message);
            }
        }

        void DrawAJ(JObject aj)
        {
            try
            {
              //  AJMSG ajbody = new AJMSG(aj);

                string ID = aj["JJDBH"].ToString();
                double X = (double)aj["ZDDWXZB"];
                double Y = (double)aj["ZDDWYZB"];

                Graphic CrimeGraphic = mapcontrol.getMapG(ID, GlobalLayers._crimepointglr);

                if (CrimeGraphic == null)
                {
                    CrimeGraphic = new Graphic();
                    CrimeGraphic.Attributes["ID"] = ID;
                    CrimeGraphic.Attributes["AJBH"] = ID;

                    CrimeGraphic.Attributes["BJSJ"] = aj["BJSJ"].ToString();
                    CrimeGraphic.Attributes["AFDZ"] = aj["SFDZ"].ToString();
                    CrimeGraphic.Attributes["BJDH"] = aj["BJDH"].ToString();
                    CrimeGraphic.Attributes["BJR"] = aj["BJRXM"].ToString();
                    CrimeGraphic.Attributes["AJLB"] = aj["BJLB"].ToString();
                    CrimeGraphic.Symbol = (Symbol)App.Current.Resources["AJSYM"];
                    CrimeGraphic.Geometry = new MapPoint(X, Y);

                    GlobalLayers._crimepointglr.Graphics.Add(CrimeGraphic);

                }
                else  // 如果已经有信息在图层里，只更新属性
                {
                    CrimeGraphic.Geometry = new MapPoint(X, Y);

                }

                if (ActivePoint == null)
                {
                    ActivePoint = new Graphic();
                    ActivePoint.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
                    GlobalLayers.ActiveLayer.Graphics.Add(ActivePoint); //  ActiveLayer.Graphics.Add(ActivePoint);
                }
                ActivePoint.Geometry = new MapPoint(X, Y);  // 绘制特效点
             //   RefreshAJ();

            }

            catch (Exception)
            {

            }
        }

    }
}
