using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zsdpmap.classes
{
    class PUBLIC_SWITCH
    {
        public static void DoSwitch(string item,string value)
        {

            switch(item)
            {
                case "警情监控"://LPY 2015-1-31 16:58:46 修改 信息 改为 警情监控
                    switch (value)
                    {
                        case "0":
                            GlobalLayers._pinfowindow.Width = 1;
                            GlobalLayers._pinfowindow.Height = 1;
                            break;
                        case "1":
                            GlobalLayers._pinfowindow.Width = 1520;
                            GlobalLayers._pinfowindow.Height = 1500;
                            break;
                    }
                    break;
                case "交通":
                    switch (value)
                    {
                        case "0":
                            GlobalLayers.TrafficLayer.Visible = false;
                            GlobalLayers.LightsLayer.Visible = false;
                            break;
                        case "1":
                            GlobalLayers.TrafficLayer.Visible = true;
                            GlobalLayers.LightsLayer.Visible = true;
                            break;
                    }
                    break;
                case "当日案件"://当日案件
                    switch (value)
                    {
                        case "0":
                            GlobalLayers.TodayAJ.Visible = false;
                            GlobalLayers.ActiveLayer.Visible = false;
                            //GlobalLayers._pinfowindow.Width = 1;//LPY 2015-11-5 11:24:05 修改 ，谢总要求重新开放
                            //GlobalLayers._pinfowindow.Height = 1;
                            break;
                        case "1":
                            GlobalLayers.TodayAJ.Visible = true;
                            GlobalLayers.ActiveLayer.Visible = true;
                            //GlobalLayers._pinfowindow.Width = 1520;
                            //GlobalLayers._pinfowindow.Height = 1500;
                            break;
                    }
                    break;
                case "警车":
                    switch (value)
                    {
                        case "0":
                            GlobalLayers._policepointglr.Visible = false;
                            GlobalLayers._offlineglr.Visible = false;
                            break;
                        case "1":
                            GlobalLayers._policepointglr.Visible = true;
                            GlobalLayers._offlineglr.Visible = true;
                            break;
                    }
                    break;
                case "警车活动":
                    switch (value)
	                {
                        case "0":
                            GlobalLayers._carinfowindow.Width = 1;
                            GlobalLayers._carinfowindow.Height = 1;
                            break;

                        case "1":
                            GlobalLayers._carinfowindow.Width = 1500;
                            GlobalLayers._carinfowindow.Height = 1000;
                            break;
	                }
                    break;
                case "避灾点":
                    switch (value)
                    {
                        case "0":
                            GlobalLayers.mfbzdlyr.Visible = false;
                            break;

                        case "1":
                            GlobalLayers.mfbzdlyr.Visible = true;
                            break;
                    }
                    break;
                default: 
                    break;
            }
        }
    }
}
