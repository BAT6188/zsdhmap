using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Toolkit;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.DataVisualization.Charting;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace zsdpmap.classes
{
    /// <summary>
    /// LPY 2014-12-25 09:18:45 添加
    /// 从MQ监听水库数据，并刷新到大屏相应位置（InfoWindow）
    /// LPY 2015-1-20 14:30:05 备注：该类已不用，仅DataConvert类和DataConvert2类被使用，因为水库信息由InfoWindow显示样式改为案件信息类似的屏幕左侧悬挂显示，已写成UserControl
    /// </summary>
    class SKInfo
    {
        #region 全局变量
        //private Dictionary<LineSeries, List<KeyValuePair<DateTime, Double>>> listLineSeries;
        private JObject skobj;
        #endregion

        public SKInfo()
        {
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
            GlobalLayers._MainMap.Dispatcher.Invoke(new DelegateRevMessage(RevMessage), msg);

            
        }

        public delegate void DelegateRevMessage(ITextMessage message);

        public void RevMessage(ITextMessage message)
        {
            GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                 new Action(
                     delegate
                     {
                         try
                         {
                             skobj = JObject.Parse(message.Text);
                             GlobalLayers.iwSK1.Anchor = new MapPoint(121.2503, 28.5389);
                             GlobalLayers.iwSK1.Content = skobj;
                             GlobalLayers.iwSK1.ContentTemplate = GlobalLayers.LayoutRoot.Resources["SKInfoWindowTemplate"] as DataTemplate;

                             byte[] bImg = Convert.FromBase64String(skobj["Pic"].ToString());
                             if (bImg != null)//显示图片
                             {
                                 BitmapImage image = new BitmapImage() { CreateOptions = BitmapCreateOptions.DelayCreation };
                                 image.BeginInit();
                                 image.StreamSource = new MemoryStream(bImg);
                                 image.EndInit();

                                 ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(GlobalLayers.iwSK1);
                                 DataTemplate myDataTemplate = GlobalLayers.iwSK1.ContentTemplate;
                                 if (myContentPresenter != null)
                                 {
                                     Image img = (Image)myDataTemplate.FindName("imgSK", myContentPresenter);
                                     img.Source = image;
                                 }
                             }

                             if (skobj.Property("HistoryData") != null)
                             {
                                 JArray jar = JArray.Parse(skobj["HistoryData"].ToString());
                                 Tuple<string, double>[] source = new Tuple<string, double>[jar.Count];
                                 for (int i = 0; i < jar.Count; i++)
                                 {
                                     JObject j = JObject.Parse(jar[i].ToString());
                                     source[i] = new Tuple<string, double>(j["TM"].ToString(), Convert.ToDouble(j["RZ"].ToString()));
                                 }
                                 ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(GlobalLayers.iwSK1);
                                 DataTemplate myDataTemplate = GlobalLayers.iwSK1.ContentTemplate;
                                 if (myContentPresenter != null)
                                 {
                                     //备用代码
                                     LineSeries lineSeries = (LineSeries)myDataTemplate.FindName("lineSeries", myContentPresenter);
                                     lineSeries.ItemsSource = source;

                                     /*
                                     LineSeries LineOne = new LineSeries();
                                     LineOne.IsSelectionEnabled = true;
                                     LineOne.IndependentValuePath = "Key";
                                     LineOne.DependentValuePath = "Value";
                                     //LineOne.DataPointStyle = this.Resources["LineDataPointStyle"] as Style;
                                     List<KeyValuePair<DateTime, Double>> valueList = new List<KeyValuePair<DateTime, Double>>();

                                     for (int i = 0; i < jar.Count; i++)
                                     {
                                         JObject j = JObject.Parse(jar[i].ToString());
                                         valueList.Add(new KeyValuePair<DateTime, double>(System.Convert.ToDateTime(j["TM"].ToString()), System.Convert.ToDouble(j["RZ"].ToString())));
                                         //valueList.Add(new KeyValuePair<DateTime, Double>(DateTime.Now.Date.AddDays(i), GetRandomInt(1400, 1600)));
                                     }
                                     LineOne.ItemsSource = valueList;
                                     Chart lineChart = (Chart)myDataTemplate.FindName("SKHistory", myContentPresenter);
                                     lineChart.Series.Clear();
                                     lineChart.Series.Add(LineOne);
                                     LineOne.Title = "历史水位";
                                     */

                                 }

                             }

                             if (skobj.Property("YLHistoryData") != null)
                             {
                                 JArray jar = JArray.Parse(skobj["YLHistoryData"].ToString());
                                 Tuple<string, double>[] source = new Tuple<string, double>[jar.Count];
                                 for (int i = 0; i < jar.Count; i++)
                                 {
                                     JObject j = JObject.Parse(jar[i].ToString());
                                     source[i] = new Tuple<string, double>(j["YLTM"].ToString(), Convert.ToDouble(j["DRP"].ToString()));
                                 }
                                 ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(GlobalLayers.iwSK1);
                                 DataTemplate myDataTemplate = GlobalLayers.iwSK1.ContentTemplate;
                                 if (myContentPresenter != null)
                                 {
                                     //备用代码
                                     LineSeries lineSeries = (LineSeries)myDataTemplate.FindName("YLlineSeries", myContentPresenter);
                                     lineSeries.ItemsSource = source;


                                 }

                             }

                             GlobalLayers.iwSK1.IsOpen = true;
                         }
                         catch (Exception ex)
                         {
                             MessageBox.Show(ex.Message);
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


    public class DataConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double tmp = double.Parse(value.ToString());
            //tmp = (tmp - 7.24) / 7.24 * 100;
            return tmp.ToString("f2");
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class DataConvert2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //double tmp = double.Parse(value.ToString());
            //tmp = (tmp - 7.24) / 7.24 * 100;//2010年7月12日的昨收盘数据为7.24
            //if (tmp > 0)
            //    return "Red";
            //else
            //    return "Green";
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    public class LineYConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Int32 intValue = System.Convert.ToInt32(value);
            //if (intValue == 0)
            //{
            //    return "0m";
            //}
            //else
            //{
            //    return value + "m";
            //}
            return (System.Convert.ToDouble(value)).ToString("f2");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    //LPY 2015-7-29 14:07:19 添加 图片路径的转换
    public class ImagePathConverter : IValueConverter
    {
        private string imageDirectory = Directory.GetCurrentDirectory()+"\\images\\";
        public string ImageDirectory
        {
            get { return imageDirectory; }
            set { imageDirectory = value; }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imagePath = Path.Combine(ImageDirectory, value.ToString());
            return new BitmapImage(new Uri(imagePath));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;//throw new NotSupportedException();
        }
    }

    public class Tuple<T1, T2>
    {
        public Tuple(T1 left, T2 right)
        {
            Left = left;
            Right = right;
        }

        public T1 Left
        {
            get;
            set;
        }
        public T2 Right
        {
            get;
            set;
        }
    }

}
