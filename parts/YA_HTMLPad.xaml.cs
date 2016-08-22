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

using System.IO;
using zsdpmap.classes;
using Newtonsoft.Json.Linq;

namespace zsdpmap.parts
{
    /// <summary>
    /// YA_HTMLPad.xaml 的交互逻辑
    /// LPY 2015-7-9 14:08:31 添加
    /// 接收客户端指令代码，根据HTML文件名称，显示相应的HTML文件
    /// </summary>
    public partial class YA_HTMLPad : UserControl
    {
        public static JObject jHtml = null;
        public string strHtmlFileName = string.Empty;

        public YA_HTMLPad()
        {
            InitializeComponent();
            faWeb.Source = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\Htmls\yasm.html");
        }

        public void ShowHtmlByName()
        {
            try
            {
                strHtmlFileName=System.IO.Directory.GetCurrentDirectory()+@"\Htmls\" + strHtmlFileName;
                if (File.Exists(strHtmlFileName))
                {                    
                    faWeb.Source = new Uri(strHtmlFileName);
                }
            }
            catch (Exception)
            {
                
            }
            
        }
    }
}
