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
using Newtonsoft.Json.Linq;

namespace zsdpmap
{
    /// <summary>
    /// YA_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class YA_INFOPAD : UserControl
    {
        public static JObject json = null;

        public YA_INFOPAD()
        {
            InitializeComponent();

//            tbContent.Text = @"城镇概况
//    院桥镇位于黄岩区南部，为省级中心镇，时全省第一批人防工作重点镇。全镇总面积80.4平方公里，设4个办事处，辖4居72个行政村，4个居委会，总人口71184人，外来人口约5万余人。院桥工业园区建设起步较早，主要有高洋、前郑、三友、繁荣、合屿、牛极等六大工业区块，现有入园企业60多家；全镇共建有各类农民专业合作社91家，其中省级规范化合作社2家，市级4家。“晨阳”牌番茄、“秀玲”牌大花丹桂、“鉴洋湖”牌果蔗、“乌岩”牌东魁杨梅等特色农产品多次在省、市农产品博览会上获奖、2012年，全镇实现共产值46.15亿元，财政总收入1.66亿元，其中地方财政收入7287万元，农民人均纯收入10948元。全镇人口共计12.1万人，其中非农业户口6040人。
//    交通情况：镇区有甬台温高速公路贯穿而过，对外主干道有是园路、原路路灯；随着104国道复线、82省道改建的开工建设，院桥成为黄岩南部交通的重要枢纽。近几年来，镇村道路得到了较大发展，村村都有可供货车进出的道路。
//    医院机构：境内有医院（卫生院）4所，卫生机构健全，卫生防疫条件大大改善，人民健康水平逐渐提高，从而使全镇的卫生出现了一派新的景象。并且大力改善农村医疗服务水平，全面推进新型农村合作医疗体系，45周岁以上人员参保率达90%以上。
//    交通运输：有载客车辆68辆，日客运量2630人，载货车辆680辆，日货运量3908吨。气象概况：院桥属亚热带季风气候，气候温和，雨量充沛，年内雨量分布不均匀，多集中在梅雨合台风季节。全年平均温度18.1℃，最高气温40.9℃，最低温度-4.1℃，年降雨量2578.6毫米，年降水天数168天，蒸发量742.2毫米，日照总时数1634.2小时。5-6月为梅雨季节，7-9月以晴天为主，夏秋之交台风活动频繁。";
        }

        public void ShowYATextByJson()
        {
            //GlobalLayers._MainMap.Dispatcher.Invoke()
            json = CMDOperator.jsonForYATextPad;
            tbTitle.Text = json["TITLE"].ToString();
            this.Tag = json["TITLE"].ToString();
            tbContent.Text = json["YAXQ"].ToString();
        }
    }
}
