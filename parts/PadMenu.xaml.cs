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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using Jovian.BigMap.classes;
using zsdpmap.classes;

namespace zsdpmap.parts
{
    /// <summary>
    /// PadMenu.xaml 的交互逻辑
    /// </summary>
    public partial class PadMenu : UserControl
    {
        //Storyboard storyboardForMenuShow = App.Current.FindResource("StoryboardForPadMenuShow") as Storyboard;
        Storyboard storyboardForMenuHidden = App.Current.FindResource("StoryboardForPadMenuHidden") as Storyboard;
        public PadMenu()
        {
            InitializeComponent();

            StartRotationStoryboard(CircleBig, 360, 0, 15);
            StartRotationStoryboard(CircleMiddle, 0, 360, 30);
            StartRotationStoryboard(CircleSmall, 360, 0, 15);


            //menuUnselectedYA.Style = this.Resources["menuSelectedYA"] as Style;
        }

        public static void StartRotationStoryboard(UIElement element,double start,double end,double duration)
        {
            RotateTransform rtf = new RotateTransform();
            element.RenderTransform = rtf;
            DoubleAnimation dbAscending = new DoubleAnimation(start, end, new Duration(TimeSpan.FromSeconds(duration)));
            Storyboard storyboard = new Storyboard();
            dbAscending.RepeatBehavior = RepeatBehavior.Forever;
            storyboard.Children.Add(dbAscending);
            Storyboard.SetTarget(dbAscending, element);
            Storyboard.SetTargetProperty(dbAscending, new PropertyPath("RenderTransform.Angle"));
            storyboard.Begin();
        }

        public void DrawMenu2ByJson(JObject json)
        {
            try
            {
                //this.BeginStoryboard(storyboardForMenuShow);//显示菜单
                if (json["LEVEL"].ToString()=="1")//一级菜单
                {
                    //InitMenu1();//1级菜单背景色初始化
                    //DropMenu2();//先清除二级菜单
                    LogHelper.WriteLog(typeof(MainWindow), "已执行：一级菜单");
                }
                if (json["LEVEL"].ToString()=="2")//二级菜单
                {
                    InitMenu1();//1级菜单背景色初始化
                    DropMenu2();//先清除二级菜单
                    double dMarginLeft = 0;
                    double dMarginTop = 0;
                    double dInterval = 250;
                    Style sMenu2 = null;

                    switch (json["MENU"].ToString())
                    {
                        case "预案查询":
                            dMarginLeft = 200;
                            dMarginTop = 1000;
                            menu1YA.Style = this.Resources["menuSelectedYA"] as Style;
                            sMenu2 = this.Resources["menu2style1a"] as Style;
                            selectYA.Visibility = Visibility.Visible;
                            break;
                        case "志愿者队伍查询":
                            dMarginLeft = 5000;
                            dMarginTop = 10;
                            menu1ZYZ.Style = this.Resources["menuSelectedZYZ"] as Style;
                            sMenu2 = this.Resources["menu2style2a"] as Style;
                            selectZYZ.Visibility = Visibility.Visible;
                            break;
                        case "民防信息查询":
                            dMarginLeft = 5500;
                            dMarginTop = 1200;
                            menu1MF.Style = this.Resources["menuSelectedMF"] as Style;
                            sMenu2 = this.Resources["menu2style2a"] as Style;
                            selectMF.Visibility = Visibility.Visible;
                            break;
                        default:
                            break;
                    }

                    JArray jar = json["SPARR"] as JArray;
                    for (int i = 0; i < jar.Count; i++)
                    {
                        Label lb = new Label() { Name = jar[i]["YAMC"].ToString(), Tag = "2-" + json["MENU"].ToString(), Height = 300, Width = 908, Style = sMenu2, Margin = new Thickness(dMarginLeft, dMarginTop + i * dInterval, 0, 0), VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Opacity = 0 };
                        DoubleAnimation daOpacity = new DoubleAnimation() { From = 0, To = 1, BeginTime = TimeSpan.FromSeconds(Convert.ToDouble(i) / 4), Duration = TimeSpan.FromSeconds(0.25), FillBehavior = FillBehavior.HoldEnd };
                        lb.BeginAnimation(UserControl.OpacityProperty, daOpacity);
                        LayoutRoot.Children.Add(lb);                        
                    }
                    LogHelper.WriteLog(typeof(MainWindow), "已执行：二级菜单");
                }
                else if (json["LEVEL"].ToString()=="3")//二级菜单被点击
                {
                    //InitMenuColorAll(rejson["LEVEL"].ToString());
                    //    ChangeMenuColorLevel2(rejson);
                    InitMenu2();//

                    for (int i = 0; i < LayoutRoot.Children.Count; i++)
                    {
                        if (typeof(Label) == LayoutRoot.Children[i].GetType())
                        {
                            Label lb = ((Label)LayoutRoot.Children[i]);
                            if (lb.Tag == null)
                                continue;
                            if (lb.Name==json["MENU"].ToString())
                            {
                                switch (lb.Tag.ToString().Split('-')[1])
                                {
                                    case "预案查询":
                                        lb.Style = this.Resources["menu2style1b"] as Style;
                                        break;
                                    case "志愿者队伍查询":
                                    case "民防信息查询":
                                        lb.Style = this.Resources["menu2style2b"] as Style;
                                        break;
                                    default:
                                        break;
                                }
                                lb.BeginStoryboard(App.Current.FindResource("StoryboardForMenuTwinkle") as Storyboard);//二级菜单闪烁
                            }
                            //Storyboard 
                            
                            this.BeginStoryboard(storyboardForMenuHidden);//隐藏菜单
                            
                        }
                    }
                    LogHelper.WriteLog(typeof(MainWindow), "已执行：三级菜单");
                }
            }
            catch (Exception)
            {
            }
        }

        public void InitMenu1()
        {
            menu1YA.Style = this.Resources["menuUnselectedYA"] as Style;
            menu1MF.Style = this.Resources["menuUnselectedMF"] as Style;
            menu1ZYZ.Style = this.Resources["menuUnselectedZYZ"] as Style;
            selectMF.Visibility = selectYA.Visibility = selectZYZ.Visibility = Visibility.Hidden;
        }

        public void DropMenu2()
        {
            try
            {
                ArrayList alNeedToDropLabel = new ArrayList();
                for (int i = 0; i < LayoutRoot.Children.Count; i++)
                {
                    if (typeof(Label) == LayoutRoot.Children[i].GetType())
                    {
                        if (((Label)LayoutRoot.Children[i]).Tag ==null)
                            continue;
                        if (((Label)LayoutRoot.Children[i]).Tag.ToString().Split('-')[0]=="2")
                            alNeedToDropLabel.Add(LayoutRoot.Children[i]);
                    }
                }
                for (int j = 0; j < alNeedToDropLabel.Count; j++)
                {
                    LayoutRoot.Children.Remove((Label)alNeedToDropLabel[j]);
                }
                alNeedToDropLabel.Clear();
            }
            catch (Exception)
            {
            }
        }

        public void InitMenu2()//二级菜单恢复底色
        {
            for (int i = 0; i < LayoutRoot.Children.Count; i++)
            {
                if (typeof(Label) == LayoutRoot.Children[i].GetType())
                {
                    Label lb = ((Label)LayoutRoot.Children[i]);
                    if (lb.Tag == null)
                        continue;
                    switch (lb.Tag.ToString().Split('-')[1])
                    {
                        case "预案查询":
                            lb.Style = this.Resources["menu2style1a"] as Style;
                            break;
                        case "志愿者队伍查询":
                        case "民防信息查询":
                            lb.Style = this.Resources["menu2style2a"] as Style;
                            break;
                        default:
                            break;
                    }
                    //if (((Label)LayoutRoot.Children[i]).Tag.ToString() == "2")
                    //    ((Label)LayoutRoot.Children[i]).Style
                }
            }
        }


    }
}
