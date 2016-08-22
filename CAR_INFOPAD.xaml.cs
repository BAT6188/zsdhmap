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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using zsdpmap.classes;
using System.ComponentModel;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Windows.Controls.DataVisualization.Charting;
using System.Globalization;
using zsdpmap.classes;

namespace zsdpmap
{
    /// <summary>
    /// CAR_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class CAR_INFOPAD : UserControl
    {
        private ObservableCollection<PoliceCarSQ> ocPoliceCarSQ = new ObservableCollection<PoliceCarSQ>();
        private JObject carObj;
        public CAR_INFOPAD()
        {
            InitializeComponent();

            try
            {
                IConnectionFactory factorySK = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"].ToString());
                IConnection connectionSK = factorySK.CreateConnection();
                connectionSK.Start();
                ISession sessionSK = connectionSK.CreateSession();

                IMessageConsumer consumerSK = sessionSK.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic(ConfigurationManager.AppSettings["TopicCAR_SQ"].ToString()));
                consumerSK.Listener += new MessageListener(ProcessSK);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("订阅GPS主题失败" + ex.Message);
            }
        }

        public void ProcessSK(IMessage message)
        {
            ITextMessage msg = (ITextMessage)message;
            g.Dispatcher.Invoke(new DelegateRevMessage(RevMessage), msg);
        }

        public delegate void DelegateRevMessage(ITextMessage message);

        public void RevMessage(ITextMessage message)
        {
            g.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                 new Action(
                     delegate
                     {
                         try
                         {
                             carObj = JObject.Parse(message.Text);
                             if (carObj["DATA"]!=null)
                             {
                                 JArray jar = JArray.Parse(carObj["DATA"].ToString());
                                 ocPoliceCarSQ.Clear();
                                 for (int i = 0; i < jar.Count; i++)
                                 {
                                     ocPoliceCarSQ.Add(new PoliceCarSQ(jar[i]["callno"].ToString(), jar[i]["sqmc"].ToString(), jar[i]["staytime"].ToString()));

                                 }
                                 listPoliceCarSQ.ItemsSource = ocPoliceCarSQ;

                             }
                         }
                         catch (Exception ex)
                         { }
                     }));
        }
    }


    //警车情况
    public class PoliceCarSQ : INotifyPropertyChanged
    {
        private string _callno;
        private string _sqmc;
        private string _staytime;

        public string callno
        {
            get { return _callno; }
            set { _callno = value; OnPropertyChanged(new PropertyChangedEventArgs("callno")); }
        }

        public string sqmc
        {
            get { return _sqmc; }
            set { _sqmc = value; OnPropertyChanged(new PropertyChangedEventArgs("sqmc")); }
        }

        public string staytime
        {
            get { return _staytime; }
            set { _staytime = value; OnPropertyChanged(new PropertyChangedEventArgs("staytime")); }
        }

        public PoliceCarSQ(string Callno, string Sqmc, string Staytime)
        {
            callno = Callno;
            sqmc = Sqmc;
            staytime = Staytime;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
