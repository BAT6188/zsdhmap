using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.IO;
using System.Threading;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Controls.DataVisualization.Charting;
using System.Globalization;
using zsdpmap.classes;

namespace zsdpmap
{
    /// <summary>
    /// SK_INFOPAD.xaml 的交互逻辑
    /// LPY 2014-12-27 19:00:14 添加 该UserControl模块
    /// 在大屏的左侧显示三个水库的水位、雨量、水库图片
    /// LPY 2014-12-28 09:34:33 修改 
    /// </summary>
    public partial class SK_INFOPAD : UserControl
    {
        #region 全局变量
        //private Dictionary<LineSeries, List<KeyValuePair<DateTime, Double>>> listLineSeries;
        private JObject skobj;
        private LineSeries LineSK1 = new LineSeries();
        private LineSeries LineYL1 = new LineSeries();
        private LineSeries LineSK2 = new LineSeries();
        private LineSeries LineYL2 = new LineSeries();
        private LineSeries LineSK3 = new LineSeries();
        private LineSeries LineYL3 = new LineSeries();
        #endregion

        public SK_INFOPAD()
        {
            
            InitializeComponent();
            try
            {
                IConnectionFactory factorySK = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());
                IConnection connectionSK = factorySK.CreateConnection();
                connectionSK.Start();
                ISession sessionSK = connectionSK.CreateSession();

                IMessageConsumer consumerSK = sessionSK.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["Topic_SK"].ToString()));
                consumerSK.Listener += new MessageListener(ProcessSK);
            }
            catch (Exception ex)
            {
                MessageBox.Show("订阅GPS主题失败" + ex.Message);
            }
        }

        public void ProcessSK(IMessage message)
        {
            ITextMessage msg = (ITextMessage)message;
            gridMain.Dispatcher.Invoke(new DelegateRevMessage(RevMessage), msg);


        }

        public delegate void DelegateRevMessage(ITextMessage message);

        public void RevMessage(ITextMessage message)
        {
            gridMain.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                 new Action(
                     delegate
                     {
                         try
                         {
                             skobj = JObject.Parse(message.Text);//总数据
                             //GlobalLayers.iwSK1.Anchor = new MapPoint(121.2503, 28.5389);
                             //GlobalLayers.iwSK1.Content = skobj;
                             //GlobalLayers.iwSK1.ContentTemplate = GlobalLayers.LayoutRoot.Resources["SKInfoWindowTemplate"] as DataTemplate;
                             //this.Content = skobj;
                             if (skobj["SK1"] != null)//水库1数据存在
                             {
                                 //gridSK1.Visibility = Visibility.Visible;
                                 spTextBlock.DataContext = skobj;//绑定水库编号、水库名称、实时水位、实时雨量到界面上。

                                 if (skobj["Pic"] != null)
                                 {
                                     byte[] bImg = Convert.FromBase64String(skobj["Pic"].ToString());
                                     if (bImg != null)
                                     {
                                         BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.DelayCreation };
                                         image.BeginInit();
                                         image.StreamSource = new MemoryStream(bImg);
                                         image.EndInit();
                                         imgSK.Source = image;//显示图片，绑定水库一的图片
                                     }
                                 }
                                 //处理水库1监测站的 近期水位 数据
                                 if (skobj.Property("HistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["HistoryData"].ToString());

                                     //zsdpmap.classes.Tuple<string, double>[] source = new zsdpmap.classes.Tuple<string, double>[jar.Count];
                                     //for (int i = 0; i < jar.Count; i++)
                                     //{
                                     //    JObject j = JObject.Parse(jar[i].ToString());
                                     //    source[i] = new zsdpmap.classes.Tuple<string, double>(j["TM"].ToString(), Convert.ToDouble(j["RZ"].ToString()));
                                     //}
                                     //lineSeries.ItemsSource = source;

                                     LineSK1.IsSelectionEnabled = true;
                                     LineSK1.IndependentValuePath = "Key";
                                     LineSK1.DependentValuePath = "Value";
                                     LineSK1.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;

                                     List<KeyValuePair<DateTime, Double>> valueList = new List<KeyValuePair<DateTime, Double>>();
                                     double SK1mini, SK1maxi, SK1Inter, SK1temp;
                                     SK1mini = double.MaxValue;
                                     SK1maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         SK1temp = System.Convert.ToDouble(j["RZ"].ToString());
                                         SK1mini = SK1mini > SK1temp ? SK1temp : SK1mini;
                                         SK1maxi = SK1maxi < SK1temp ? SK1temp : SK1maxi;
                                         valueList.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["DT"].ToString() + " " + j["TM"].ToString()), SK1temp));
                                         //valueList.Add(new KeyValuePair<DateTime, Double>(DateTime.Now.Date.AddDays(i), GetRandomInt(1400, 1600)));
                                     }
                                     SK1Inter = Math.Round(SK1maxi - SK1mini, 2) > 0 ? Math.Round(SK1maxi - SK1mini, 2) : 1;
                                     YAxisSK1.Maximum = SK1maxi + SK1Inter;
                                     YAxisSK1.Minimum = SK1mini - SK1Inter;
                                     YAxisSK1.Interval = SK1Inter;
                                     LineSK1.ItemsSource = valueList;
                                     SKHistory.Series.Clear();
                                     SKHistory.Series.Add(LineSK1);
                                     LineSK1.Title = "";
                                 }

                                 //处理水库1监测站的 近期雨量 数据
                                 if (skobj.Property("YLHistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["YLHistoryData"].ToString());

                                     LineYL1.IsSelectionEnabled = true;
                                     LineYL1.IndependentValuePath = "Key";
                                     LineYL1.DependentValuePath = "Value";
                                     LineYL1.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;

                                     List<KeyValuePair<DateTime, Double>> valueListYL = new List<KeyValuePair<DateTime, Double>>();
                                     double mini, maxi, inter, temp;
                                     mini = double.MaxValue;
                                     maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         temp = System.Convert.ToDouble(j["DRP"].ToString());
                                         mini = mini > temp ? temp : mini;
                                         maxi = maxi < temp ? temp : maxi;
                                         valueListYL.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["YLDT"].ToString() + " " + j["YLTM"].ToString()), temp));
                                         //valueList.Add(new KeyValuePair<DateTime, Double>(DateTime.Now.Date.AddDays(i), GetRandomInt(1400, 1600)));
                                     }
                                     inter = Math.Round(maxi - mini, 2) > 0 ? Math.Round(maxi - mini, 2) : 1;
                                     YAxisYL1.Maximum = maxi + inter;
                                     YAxisYL1.Minimum = mini - inter;
                                     YAxisYL1.Interval = inter;
                                     LineYL1.ItemsSource = valueListYL;
                                     YLHistory.Series.Clear();
                                     YLHistory.Series.Add(LineYL1);
                                     LineYL1.Title = "";

                                     //zsdpmap.classes.Tuple<string, double>[] source = new zsdpmap.classes.Tuple<string, double>[jar.Count];
                                     //for (int i = 0; i < jar.Count; i++)
                                     //{
                                     //    JObject j = JObject.Parse(jar[i].ToString());
                                     //    source[i] = new zsdpmap.classes.Tuple<string, double>(j["YLTM"].ToString(), Convert.ToDouble(j["DRP"].ToString()));
                                     //}
                                     //ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(GlobalLayers.iwSK1);
                                     //DataTemplate myDataTemplate = GlobalLayers.iwSK1.ContentTemplate;
                                     //if (myContentPresenter != null)
                                     //{
                                     //备用代码
                                     //LineSeries lineSeries = (LineSeries)myDataTemplate.FindName("YLlineSeries", myContentPresenter);
                                     //YLlineSeries.ItemsSource = source;
                                     //}
                                 }
                             }
                             else
                             {
                                 //gridSK1.Visibility = Visibility.Hidden;
                                 
                             }

                             #region 水库2的信息处理
                             if (skobj["SK2"]!=null)
                             {
                                 //gridSK2.Visibility = Visibility.Visible;//将该水库信息模块显示出来

                                 spTextBlockSK2.DataContext = skobj;//绑定水库编号、水库名称、实时水位、实时雨量到界面上。

                                 if (skobj["Pic"] != null)
                                 {
                                     byte[] bImg = Convert.FromBase64String(skobj["Pic"].ToString());
                                     if (bImg != null)//显示图片，绑定水库二的图片
                                     {
                                         BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.DelayCreation };
                                         image.BeginInit();
                                         image.StreamSource = new MemoryStream(bImg);
                                         image.EndInit();
                                         imgSK2.Source = image;
                                     }
                                 }
                                 //处理水库2监测站的 近期水位 数据
                                 if (skobj.Property("HistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["HistoryData"].ToString());

                                     LineSK2.IsSelectionEnabled = true;
                                     LineSK2.IndependentValuePath = "Key";
                                     LineSK2.DependentValuePath = "Value";
                                     LineSK2.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;

                                     List<KeyValuePair<DateTime, Double>> valueList = new List<KeyValuePair<DateTime, Double>>();
                                     double SK2mini, SK2maxi, SK2Inter, SK2temp;
                                     SK2mini = double.MaxValue;
                                     SK2maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         SK2temp = System.Convert.ToDouble(j["RZ"].ToString());
                                         SK2mini = SK2mini > SK2temp ? SK2temp : SK2mini;
                                         SK2maxi = SK2maxi < SK2temp ? SK2temp : SK2maxi;
                                         valueList.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["DT"].ToString() + " " + j["TM"].ToString()), SK2temp));
                                     }
                                     SK2Inter = Math.Round(SK2maxi - SK2mini, 2) > 0 ? Math.Round(SK2maxi - SK2mini, 2) : 1;
                                     YAxisSK2.Maximum = SK2maxi + SK2Inter;
                                     YAxisSK2.Minimum = SK2mini - SK2Inter;
                                     YAxisSK2.Interval = SK2Inter;
                                     LineSK2.ItemsSource = valueList;
                                     SK2History.Series.Clear();
                                     SK2History.Series.Add(LineSK2);
                                     LineSK2.Title = "";
                                 }

                                 //处理水库2监测站的 近期雨量 数据
                                 if (skobj.Property("YLHistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["YLHistoryData"].ToString());

                                     LineYL2.IsSelectionEnabled = true;
                                     LineYL2.IndependentValuePath = "Key";
                                     LineYL2.DependentValuePath = "Value";
                                     LineYL2.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;
                                     List<KeyValuePair<DateTime, Double>> valueListYL = new List<KeyValuePair<DateTime, Double>>();
                                     double mini, maxi, inter, temp;
                                     mini = double.MaxValue;
                                     maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         temp = System.Convert.ToDouble(j["DRP"].ToString());
                                         mini = mini > temp ? temp : mini;
                                         maxi = maxi < temp ? temp : maxi;
                                         valueListYL.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["YLDT"].ToString() + " " + j["YLTM"].ToString()), temp));
                                     }
                                     inter = Math.Round(maxi - mini, 2) > 0 ? Math.Round(maxi - mini, 2) : 1;
                                     YAxisYL2.Maximum = maxi + inter;
                                     YAxisYL2.Minimum = mini - inter;
                                     YAxisYL2.Interval = inter * 2;
                                     LineYL2.ItemsSource = valueListYL;
                                     YL2History.Series.Clear();
                                     YL2History.Series.Add(LineYL2);
                                     LineYL2.Title = "";
                                 }
                             }
                             else
                             {
                                 //gridSK2.Visibility = Visibility.Collapsed;
                             }
                             #endregion

                             #region 水库3的信息处理
                             if (skobj["SK3"] != null)
                             {
                                 //gridSK3.Visibility = Visibility.Visible;//将该水库信息显示出来

                                 spTextBlockSK3.DataContext = skobj;//绑定水库编号、水库名称、实时水位、实时雨量到界面上。

                                 if (skobj["Pic"] != null)
                                 {
                                     byte[] bImg = Convert.FromBase64String(skobj["Pic"].ToString());
                                     if (bImg != null)//显示图片，绑定水库二的图片
                                     {
                                         BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.DelayCreation };
                                         image.BeginInit();
                                         image.StreamSource = new MemoryStream(bImg);
                                         image.EndInit();
                                         imgSK3.Source = image;
                                     }
                                 }
                                 //处理水库3监测站的 近期水位 数据
                                 if (skobj.Property("HistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["HistoryData"].ToString());

                                     LineSK3.IsSelectionEnabled = true;
                                     LineSK3.IndependentValuePath = "Key";
                                     LineSK3.DependentValuePath = "Value";
                                     LineSK3.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;

                                     List<KeyValuePair<DateTime, Double>> valueList = new List<KeyValuePair<DateTime, Double>>();
                                     double SK3mini, SK3maxi, SK3Inter, SK3temp;
                                     SK3mini = double.MaxValue;
                                     SK3maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         SK3temp = System.Convert.ToDouble(j["RZ"].ToString());
                                         SK3mini = SK3mini > SK3temp ? SK3temp : SK3mini;
                                         SK3maxi = SK3maxi < SK3temp ? SK3temp : SK3maxi;
                                         valueList.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["DT"].ToString() + " " + j["TM"].ToString()), SK3temp));
                                     }
                                     SK3Inter = Math.Round(SK3maxi - SK3mini, 2) > 0 ? Math.Round(SK3maxi - SK3mini, 2) : 1;
                                     YAxisSK3.Maximum = SK3maxi + SK3Inter;
                                     YAxisSK3.Minimum = SK3mini - SK3Inter;
                                     YAxisSK3.Interval = SK3Inter;
                                     LineSK3.ItemsSource = valueList;
                                     SK3History.Series.Clear();
                                     SK3History.Series.Add(LineSK3);
                                     LineSK3.Title = "";
                                 }

                                 //处理水库3监测站的 近期雨量 数据
                                 if (skobj.Property("YLHistoryData") != null)
                                 {
                                     JArray jar = JArray.Parse(skobj["YLHistoryData"].ToString());

                                     LineYL3.IsSelectionEnabled = true;
                                     LineYL3.IndependentValuePath = "Key";
                                     LineYL3.DependentValuePath = "Value";
                                     LineYL3.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;
                                     List<KeyValuePair<DateTime, Double>> valueListYL = new List<KeyValuePair<DateTime, Double>>();
                                     double mini, maxi, inter, temp;
                                     mini = double.MaxValue;
                                     maxi = double.MinValue;
                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         temp = System.Convert.ToDouble(j["DRP"].ToString());
                                         mini = mini > temp ? temp : mini;
                                         maxi = maxi < temp ? temp : maxi;
                                         valueListYL.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["YLDT"].ToString() + " " + j["YLTM"].ToString()), temp));
                                     }
                                     inter = Math.Round(maxi - mini, 2) > 0 ? Math.Round(maxi - mini, 2) : 1;
                                     YAxisYL3.Maximum = maxi + inter;
                                     YAxisYL3.Minimum = mini - inter;
                                     YAxisYL3.Interval = inter * 2;
                                     LineYL3.ItemsSource = valueListYL;
                                     YL3History.Series.Clear();
                                     YL3History.Series.Add(LineYL3);
                                     LineYL3.Title = "";
                                 }
                             }
                             else
                             {
                                 //gridSK3.Visibility = Visibility.Hidden;
                             }
                             #endregion
                             //GlobalLayers.iwSK1.IsOpen = true;
                         }
                         catch (Exception ex)
                         {
                         }
                     }));

        }


        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
    }
}
