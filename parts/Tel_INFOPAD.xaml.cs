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
using zsdpmap.classes;


using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System.Threading;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace zsdpmap.parts
{
    /// <summary>
    /// Tel_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class Tel_INFOPAD : UserControl
    {
        public Tel_INFOPAD()
        {
            InitializeComponent();

            try
            {
                IConnectionFactory factory = new ConnectionFactory(ConfigurationManager.AppSettings["MQ"]);
                IConnection connection = factory.CreateConnection();
                connection.Start();
                ISession session = connection.CreateSession();
                IMessageConsumer consumer = session.CreateConsumer(new Apache.NMS.ActiveMQ.Commands.ActiveMQTopic("TelState"));
                consumer.Listener += consumer_Listener;
            }
            catch (Exception)
            {
                
            }
        }

        void consumer_Listener(IMessage message)
        {
            //throw new NotImplementedException();
            ITextMessage msg = (ITextMessage)message;
            LayoutRoot.Dispatcher.Invoke(new Action(() => {
                JObject json = JObject.Parse(msg.Text);
                if (json.Property("State")!=null)
                {
                    tbDial.Text = json["State"].ToString();
                }
                
            }));
        }
    }
}
