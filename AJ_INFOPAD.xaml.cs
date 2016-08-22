using Apache.NMS;
using Apache.NMS.ActiveMQ;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
using System.Collections.ObjectModel;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Threading;

namespace zsdpmap
{
    /// <summary>
    /// AJ_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class AJ_INFOPAD : UserControl
    {
        class DataSources
        {
            public static DateTime NewestBJSJ = new DateTime(2000,1,1);
            public static string LASTJJDBH = "00000000000000";
            static int MAXQUEUE = 5;
            public static ObservableCollection<Information> AJQUEUE = new ObservableCollection<Information>();  // 原先为处理EditItem 例外，后改这方法后发现关键还是绑定项目需要设置为ReadOnly
        //    public static LinkedList<Information> AJQUEUE = new LinkedList<Information>();
            public static LinkedList<string> New2000 = new LinkedList<string>();
          //  public static ConcurrentQueue<Information> AJQUEUE = new ConcurrentQueue<Information>();
            
            public static DataGrid GlobalAJ = null;
            // 循环
            public static void LoopQueue(Information info)
            {
                if (AJQUEUE.Count == MAXQUEUE)
                    AJQUEUE.RemoveAt(MAXQUEUE-1);  //  RemoveLast();                   
                AJQUEUE.Insert(0, info); // AddFirst(info);
            }
            public static bool CheckUpdate(string BJBH)
            {
                if (New2000.Contains(BJBH))
                    return true;

                if (New2000.Count > 2000)
                    New2000.RemoveLast();
                New2000.AddFirst(BJBH);
                    return false;
                
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
            double _X;
            double _Y;
            string _ID;
            public Information(JObject obj)
            {
                try
                {
                    _ID = obj["JJDBH"].ToString();
                    _X = (double)obj["ZDDWXZB"];
                    _Y = (double)obj["ZDDWYZB"];
                    this._AJBH = obj["JJDBH"].ToString();
                    this._BJSJ = obj["BJSJ"].ToString();
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

            public double X
            {
                get
                { return _X; }
            }

            public double Y
            {
                get
                {
                    return _Y;
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

      //  static Dictionary<int, double> AJTJ = new Dictionary<int, double>();

        void dTimer_Tick(object sender, EventArgs e)
        {
            ReCalc();
        }

        void dTimer_Tick_Test(object sender, EventArgs e)
        {
            Redraw();
        }
        void Tick()
        {
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 5000);   // 5S 刷新
            dTimer.Start();
        }

        void TickTest()
        {
            DispatcherTimer dTimer = new DispatcherTimer();
            dTimer.Tick += new EventHandler(dTimer_Tick_Test);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);   // 5S 刷新
            dTimer.Start();
        }
        public AJ_INFOPAD()
        {
            InitializeComponent();

            RefreshAJ();        // 刷新案件图层
            Tick(); // 定时刷新统计信息
    //        TickTest();
            //        Thread RecalcAJ = new Thread(new ThreadStart(RefreshAJ_10S));
            // 刷新案件信息图层并更新统计直方图 间隔 5S 

/*
            Task RecalcAJ = new Task(RefreshAJ_10S);
            RecalcAJ.Start();
*/
            Subscribe();        // 订阅案件信息

        }

        private delegate void DelegateRedrawChart();
        private void Redraw()
        {
            Random R = new Random();

            KeyValuePair<int, int>[] DataValues;
            DataValues = new KeyValuePair<int, int>[24];
            for (int i = 0; i < 24; i++)
            {
                DataValues[i] = new KeyValuePair<int, int>(i, R.Next(1, 20));

            }

          
            ((ColumnSeries)AJCOUNT).ItemsSource = DataValues;
        }
        private void TestCalc()
        {
            Redraw();
            /*
            DelegateRedrawChart Rr = Redraw;
            this.Dispatcher.Invoke(Rr);*/
        }
        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            //           MessageBox.Show("Query failed: " + args.Error);
        }

        private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            KeyValuePair<int, int>[] DataValues;
            DataValues = new KeyValuePair<int, int>[24];
            int[] CalcOf24Hour = new int[24];
            try
            {

                for (int i = 0; i < 24; i++)
                    CalcOf24Hour[i] = 0;                
                FeatureSet featureSet = args.FeatureSet;
                
                if (featureSet != null && featureSet.Features.Count > 0)
                {
                    foreach (Graphic feature in featureSet.Features)    // 统计当天案件数
                    {
                        DateTime bjsj = DateTime.Parse(feature.Attributes["BJSJ"].ToString());
                        CalcOf24Hour[bjsj.Hour] += 1;
                    }                   
                }
                for (int i = 0; i < 24; i++)
                    DataValues[i] = new KeyValuePair<int, int>(i, CalcOf24Hour[i]);
                ((ColumnSeries)AJCOUNT).ItemsSource = DataValues;

           /*     AJCOUNT.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                new Action(
                delegate
                {
                    AJCOUNT.ItemsSource = AJTJ;
                    AJCOUNT.Refresh();
                }));
            */
            }
            catch (Exception)
            {
            }
        }

        // 刷新案件统计直方图
        void ReCalc()
        {
            try
            {
                QueryTask _queryTask;
                _queryTask = new QueryTask(ConfigurationManager.AppSettings["TODAYAJ"].ToString());
                _queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
                _queryTask.Failed += QueryTask_Failed;

                string datestr = DateTime.Now.ToString("yyyy-MM-dd");

                Query _query = new Query();
                _query.ReturnGeometry = false;
                //_query.Where = "GXDWBH like '330902%' and jjsj >= " + "date '" + datestr + "'";
                _query.Where = "bjsj >= " + "date '" + datestr + "'";
                _query.OutFields.Add("*");
                _queryTask.ExecuteAsync(_query);
            } catch (Exception)
            {

            }
        }

        void RefreshAJ() // 刷新案件图层
        {
            GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                 new Action(
                     delegate
                     {
                         try
                         {
                             if (GlobalLayers.TodayAJ != null)
                             {
                                 //GlobalLayers.TodayAJ.Visible = false;
                                

                                 //           GlobalLayers.TodayAJ.Url = ConfigurationManager.AppSettings["TODAYAJ"].ToString();
                                 DateTime today = DateTime.Today;

                                 //  string DateStart = today.Date.ToString();

                                 DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();

                                 dtFormat.ShortDatePattern = "yyyy/MM/dd";

                                 string DateStart = today.Date.ToString("dd/MMM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                                 string DateEnd = today.AddDays(1).Date.ToString("dd/MMM/yyyy", CultureInfo.CreateSpecificCulture("en-US")); // 第二天的0:0:0
                                // 下面语句只筛选定海的警情

                                 string datestr = DateTime.Now.ToString("yyyy-MM-dd");

                            //     GlobalLayers.TodayAJ.Where = "GXDWBH like '330902%' and jjsj >= " + "date '" + datestr + "'";
                                 GlobalLayers.TodayAJ.Where = "bjsj >= " + "date '" + datestr + "'";
                            
                                 GlobalLayers.TodayAJ.OutFields = new OutFields { "*" };
                                 GlobalLayers.TodayAJ.Mode = FeatureLayer.QueryMode.OnDemand;
                                 //GlobalLayers.TodayAJ.Mode = FeatureLayer.QueryMode.Snapshot;// 2015-1-14 14:52:35 修改
                                 //LPY 2015-1-20 14:40:12 修改：按要求作如下修改
                                 if (GlobalLayers.TodayAJ.Renderer==null)
                                 {
                                     SimpleRenderer render = new SimpleRenderer();
                                     render.Symbol = (Symbol)App.Current.Resources["AJSYM"];
                                     GlobalLayers.TodayAJ.Renderer = render;
                                 }
                                 GlobalLayers.TodayAJ.Update();
                                 GlobalLayers.TodayAJ.Refresh();
                                 //GlobalLayers.TodayAJ.Visible = true;


                             }
                         }
                         catch (Exception)
                         {

                         }
                     }));
        }
            
        
        void RefreshAJ_10S()
        {
            while(true)
            {

                ReCalc();
                Thread.Sleep(100);
            }
        }
        //处理案件信息订阅
        public void Subscribe()
        {
                try
                {
                    IConnectionFactory factory;

                    factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());   // 连接MQ数据发布服务器
                    IConnection connection = factory.CreateConnection();
                    connection.Start();
                    session = connection.CreateSession();
                    consumer = session.CreateDurableConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_AJ"].ToString()), "GQY", null, false);

                    consumer.Listener += new MessageListener(consumer_Listener);


                }
                catch(Exception e)
                {
                    MessageBox.Show("订阅案件主题失败"+e.Message);
                }
                
        }

        // 收到案件信息的处理
        void consumer_Listener(IMessage message)
        {
            try
            {

                ITextMessage msg = (ITextMessage)message;
                JObject obj;

                obj = JObject.Parse(msg.Text);


                aj = new Information(obj);
                if (aj == null)
                    return;
 
                
                     //       if (DataSources.CheckUpdate(aj.AJBH)) // 消除同一个案件编号修改后重复推送
                     //           return;


                            // 判断报警时间和记录的最新报警时间，如果比以前收到的旧，就跳过。用于消除指挥中心更正警单
                            // 否则更新报警时间
                            // 
                            try
                            {
                                if (DateTime.Parse(aj.BJSJ) <= DataSources.NewestBJSJ)
                                    return;
                                else
                                    DataSources.NewestBJSJ = DateTime.Parse(aj.BJSJ);
                            } catch(Exception)
                            {
                                return; // 时间异常的也直接返回
                            }

                            RefreshAJ();        // 更新案件图层，并刷新统计值
                            // 刷新列表和DELTAIL
                            AJLST.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        DataSources.LoopQueue(aj);  // 刷新最新警情队列
                                        
                                        AJLST.ItemsSource = null;                   // 先置空再绑定是 DATAGRID 的刷新数据方法
                                        AJLST.ItemsSource = DataSources.AJQUEUE;    // 警情队列更新
                                        JJDBH.Text = aj.AJBH;                       // 最新的一条警情详细
                                        AFDZ.Text = aj.AFDZ;
                                        BJLB.Text = aj.BJLXMC;
                                        BJNR.Text = aj.BJNR;
                                        BJDH.Text = aj.BJDH;
                                        BJSJ.Text = aj.BJSJ;
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
                Console.WriteLine(e.Message);
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

                if (ActivePoint == null)
                {
                    ActivePoint = new Graphic();
                    ActivePoint.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
                    GlobalLayers.ActiveLayer.Graphics.Add(ActivePoint); //  ActiveLayer.Graphics.Add(ActivePoint);
                }
                ActivePoint.Geometry = new MapPoint(X, Y);  // 绘制特效点  
                



/*
                InfoWindow window = null;

                Thickness thickness = new Thickness(5, 5, 5, 5);

                MapTip maptip = new MapTip();
                maptip.Title = "AAAAAA";
                maptip.Padding = thickness;
                maptip.BorderBrush = GlobalLayers.LayoutRoot.Resources["PanelGradient"] as Brush;
                CrimeGraphic.MapTip = maptip;

                window = Take_A_IW();
                //         GlobalLayers.LayoutRoot.Children.Remove(window);
                window.Anchor = CrimeGraphic.Geometry as MapPoint;
                window.CornerRadius = 20;
                window.Padding = thickness;
                window.Background = GlobalLayers.LayoutRoot.Resources["PanelGradient"] as Brush;
                window.Map = GlobalLayers._MainMap;
                window.Placement = InfoWindow.PlacementMode.Auto;
                window.ContentTemplate = GlobalLayers.LayoutRoot.Resources["AJDataTemplate"] as DataTemplate;
                window.Content = aj;

                window.IsOpen = false; //  true;

                */



            }

            catch (Exception)
            {

            }
        }




    }
}
