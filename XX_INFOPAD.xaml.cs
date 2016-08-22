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

namespace zsdpmap
{
    /// <summary>
    /// XX_INFOPAD.xaml 的交互逻辑
    /// </summary>
    public partial class XX_INFOPAD : UserControl
    {
        private ObservableCollection<PeopleWorkInfo> ocPeopleWorkInfo = new ObservableCollection<PeopleWorkInfo>();
        private ObservableCollection<ZYZTeam> ocZYZTeam = new ObservableCollection<ZYZTeam>();
        private ObservableCollection<CityPoliceStat> ocCityPoliceStat = new ObservableCollection<CityPoliceStat>();
        private ObservableCollection<CityPoliceStat> ocCityPoliceStat2 = new ObservableCollection<CityPoliceStat>();
        public XX_INFOPAD()
        {
            InitializeComponent();
            
            //FillData();//填充网格数据
        }

        public void FillData()
        {
            HideListViews();
            JObject xxJsonInfo = classes.CMDOperator.xxJsonInfo;
            tbTitle.Text = xxJsonInfo["title"].ToString();
            //this.Tag=
            //if ( xxJsonInfo["title"].ToString()=="人员工作统计")
            //{
                
            //}
            switch (xxJsonInfo["title"].ToString())
            {
                case "人员工作统计":
                    if (xxJsonInfo["RYGZ"]["result"].ToString() != "")
                    {
                        tbTitle.Text = xxJsonInfo["title"].ToString() + "-" + xxJsonInfo["ryxm"].ToString();
                        JArray jar = JArray.Parse(xxJsonInfo["RYGZ"]["result"].ToString());
                        ocPeopleWorkInfo.Clear();
                        for (int i = 0; i < jar.Count; i++)
                        {
                            ocPeopleWorkInfo.Add(new PeopleWorkInfo(jar[i]["jssj"].ToString(), jar[i]["jsbt"].ToString(), GetYWLB(jar[i]["ywlb"].ToString()), jar[i]["jsdz"].ToString()));
                        }
                        listPeopleWorkInfo.ItemsSource = ocPeopleWorkInfo;
                        listPeopleWorkInfo.Visibility = Visibility.Visible;
                    }
                    break;
                case "院桥镇人防（民防）志愿者队伍抢险抢修分队花名册":
                case "院桥镇人防（民防）志愿者队伍消防分队花名册":
                case "院桥镇人防（民防）志愿者队伍宣传教育分队花名册":
                case "院桥镇人防（民防）志愿者队伍组织疏散分队花名册":
                case "院桥镇人防（民防）志愿者队伍医疗救护分队花名册":
                    if (xxJsonInfo["RYGZ"]["aaData"].ToString() != "")
                    {
                        JArray jar = JArray.Parse(xxJsonInfo["RYGZ"]["aaData"].ToString());
                        ocZYZTeam.Clear();
                        for (int i = 0; i < jar.Count; i++)
                        {
                            ocZYZTeam.Add(new ZYZTeam(jar[i]["XM"].ToString(), jar[i]["XB"].ToString(), jar[i]["SFZH"].ToString(), jar[i]["GZDW"].ToString(), jar[i]["ZW"].ToString(), jar[i]["DT"].ToString(), jar[i]["WHCD"].ToString(), jar[i]["LXDH"].ToString(), jar[i]["DH"].ToString()));
                        }
                        listZYZQXQXFDHMC.ItemsSource = ocZYZTeam;
                        listZYZQXQXFDHMC.Visibility = Visibility.Visible;
                    }
                    break;
                case "全市警务状况":
                    if (xxJsonInfo["RYGZ"].ToString() != "")
                    {
                        JArray jar = JArray.Parse(xxJsonInfo["RYGZ"].ToString());
                        ocCityPoliceStat.Clear();
                        for (int i = 0; i < jar.Count; i++)
                        {
                            ocCityPoliceStat.Add(new CityPoliceStat(jar[i]["mc"].ToString(), jar[i]["lmpc"].ToString(), jar[i]["czfzf"].ToString(), "", jar[i]["rysl"].ToString(), jar[i]["clsl"].ToString(), "",""));
                        }
                        listCityPoliceStat.ItemsSource = ocCityPoliceStat;
                        listCityPoliceStat.Visibility = Visibility.Visible;
                    }
                    break;
                case "全市警务状况2":
                    tbTitle.Text = "全市警务状况";
                    if (xxJsonInfo["RYGZ"].ToString() != "")
                    {

                        JArray jar = JArray.Parse(xxJsonInfo["RYGZ"].ToString());
                        ocCityPoliceStat2.Clear();
                        for (int i = 0; i < jar.Count; i++)
                        {
                            ocCityPoliceStat2.Add(new CityPoliceStat(jar[i]["mc"].ToString(), jar[i]["lmpc"].ToString(), jar[i]["czfzf"].ToString(), "", jar[i]["rysl"].ToString(), jar[i]["clsl"].ToString(), ""));
                        }
                        listCityPoliceStat2.ItemsSource = ocCityPoliceStat2;
                        listCityPoliceStat2.Visibility = Visibility.Visible;
                    }
                    break;
                default:
                    break;
            }
            //this.Tag = tbTitle.Text;
            
            
        }

        //获得业务类别
        private string GetYWLB(string ywlb)
        {
            string tempYWLB = "";
            switch (ywlb)
            {
                case "1":
                    tempYWLB = "盘查";
                    break;
                case "2":
                    tempYWLB = "房管";
                    break;
                case "3":
                    tempYWLB = "船管";
                    break;
                default:
                    tempYWLB = "";
                    break;
            }
            return tempYWLB;
        }

        private void HideListViews()
        {
            listCityPoliceStat2.Visibility=
            listCityPoliceStat.Visibility=
            listPeopleWorkInfo.Visibility = 
                listZYZQXQXFDHMC.Visibility =
                Visibility.Collapsed;
            

        }


    }

    //人员工作信息    
    public class PeopleWorkInfo:INotifyPropertyChanged
    {
        private string _jssj;
        private string _jsnr;
        private string _ywlb;
        private string _jsdz;

        public string jssj
        {
            get { return _jssj; }
            set { 
                _jssj = value;
                OnPropertyChanged(new PropertyChangedEventArgs("jssj"));               
            }
        }

        public string jsnr
        {
            get { return _jsnr; }
            set
            {
                _jsnr = value; 
                OnPropertyChanged(new PropertyChangedEventArgs("jsnr")); 
            }
        }

        public string ywlb
        {
            get { return _ywlb; }
            set
            {
                _ywlb = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ywlb"));
            }
        }

        public string jsdz
        { 
            get { return _jsdz; }
            set
            {
                _jsdz = value;
                OnPropertyChanged(new PropertyChangedEventArgs("jsdz"));
            }
        }

        public PeopleWorkInfo(string Jssj, string Jsnr, string Ywlb, string Jsdz)
        {
            jssj = Jssj;
            jsnr = Jsnr;
            ywlb = Ywlb;
            jsdz = Jsdz;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);                
        }
    }

    //院桥镇人防（民防）志愿者队伍抢险抢修分队花名册
    public class ZYZTeam : INotifyPropertyChanged
    {
        private string _xm;//姓名
        private string _xb;//性别
        private string _sfzh;//身份证号
        private string _gzdw;//工作单位
        private string _zw;//职务
        private string _dt;//党团
        private string _whcd;//文化程度
        private string _lxdh;//联系电话
        private string _dh;//短号

        public ZYZTeam(string Xm, string Xb, string Sfzh, string Gzdw, string Zw, string Dt, string Whcd, string Lxdh, string Dh)
        {
            XM = Xm;
            XB = Xb;
            SFZH = Sfzh;
            GZDW = Gzdw;
            ZW = Zw;
            DT = Dt;
            WHCD = Whcd;
            LXDH = Lxdh;
            DH = Dh;
        }

        public string DH
        {
            get { return _dh; }
            set
            {
                _dh = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DH"));
            }
        }

        public string LXDH
        {
            get { return _lxdh; }
            set
            {
                _lxdh = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LXDH"));
            }
        }

        public string WHCD
        {
            get { return _whcd; }
            set
            {
                _whcd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("WHCD"));
            }
        }

        public string DT
        {
            get { return _dt; }
            set
            {
                _dt = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DT"));
            }
        }

        public string GZDW
        {
            get { return _gzdw; }
            set
            {
                _gzdw = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GZDW"));
            }
        }

        public string ZW
        {
            get { return _zw; }
            set
            {
                _zw = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ZW"));
            }
        }

        public string XM
        {
            get { return _xm; }
            set
            {
                _xm = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XM"));
            }
        }

        public string XB
        {
            get { return _xb; }
            set
            {
                _xb = value;
                OnPropertyChanged(new PropertyChangedEventArgs("XB"));
            }
        }

        public string SFZH
        {
            get { return _sfzh; }
            set
            {
                _sfzh = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SFZH"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }

    //全市警务状况
    public class CityPoliceStat : INotifyPropertyChanged
    {
        private string _jgmc;//机构名称
        private string _lmpc;//路面盘查
        private string _czfzf;//出租房走访
        private string _wbzf;//网吧走访
        private string _rysl;//人员数量
        private string _clsl;//车辆数量
        private string _zdrys;//重点人员数

        private string _jyxm;//警员姓名

        public string jgmc//机构名称
        {
            get { return _jgmc; }
            set { _jgmc = value; OnPropertyChanged(new PropertyChangedEventArgs("jgmc")); }            
        }
        public string lmpc//路面盘查
        {
            get { return _lmpc; }
            set { _lmpc = value; OnPropertyChanged(new PropertyChangedEventArgs("lmpc")); }
        }
        public string czfzf//出租房走访
        {
            get { return _czfzf; }
            set { _czfzf = value; OnPropertyChanged(new PropertyChangedEventArgs("czfzf")); }
        }
        public string wbzf//网吧走访
        {
            get { return _wbzf; }
            set { _wbzf = value; OnPropertyChanged(new PropertyChangedEventArgs("wbzf")); }
        }
        public string rysl//人员数量
        {
            get { return _rysl; }
            set { _rysl = value; OnPropertyChanged(new PropertyChangedEventArgs("rysl")); }
        }
        public string clsl//车辆数量
        {
            get { return _clsl; }
            set { _clsl = value; OnPropertyChanged(new PropertyChangedEventArgs("clsl")); }
        }
        public string zdrys//重点人员数
        {
            get { return _zdrys; }
            set { _zdrys = value; OnPropertyChanged(new PropertyChangedEventArgs("zdrys")); }
        }

        public string jyxm//警员姓名
        {
            get { return _jyxm; }
            set { _jyxm = value; OnPropertyChanged(new PropertyChangedEventArgs("jyxm")); }
        }

        //带警员姓名
        public CityPoliceStat(string Jyxm, string Lmpc, string Czfzf, string Wbzf, string Rysl, string Clsl, string Zdrys)
        {
            jyxm = Jyxm;
            lmpc = Lmpc;
            czfzf = Czfzf;
            wbzf = Wbzf;
            rysl = Rysl;
            clsl = Clsl;
            zdrys = Zdrys;
        }

        //带机构名称
        public CityPoliceStat(string Jgmc, string Lmpc, string Czfzf, string Wbzf, string Rysl, string Clsl, string Zdrys,string temp)
        {
            jgmc = Jgmc;
            lmpc = Lmpc;
            czfzf = Czfzf;
            wbzf = Wbzf;
            rysl = Rysl;
            clsl = Clsl;
            zdrys = Zdrys;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }

    


}
