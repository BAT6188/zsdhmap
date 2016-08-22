using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace zsdpmap.classes
{
    public class HeatMapLayercontrol
    {
        mapcontrol _mapcontrol = new mapcontrol();
        public static int HeatMapIntensity = Convert.ToInt32(ConfigurationManager.AppSettings["HeatMapIntensity"].ToString());//设置热力图的情况

        #region 管理HeatMapLayer
        // 热力图处理时间比较长，要测试热力图打开到响应的时长！！！
        public string AddHeatMapLayer(JObject jsoncommand, Map currentMap)
        {
            try
            {
                string layername = jsoncommand["LAYER"].ToString();
                string layerurl = ConfigurationManager.AppSettings[layername].ToString();
                myheatmaplayer phmlyr = _mapcontrol.getHmLyr(layername, currentMap);
                //phmlyr.Intensity = 100;
                if (phmlyr == null)
                {
                    phmlyr = new myheatmaplayer();
                    phmlyr.ID = layername;
                    phmlyr.setfilter = jsoncommand["FILTER"].ToString();//LPY 2015-7-30 17:00:25 修改 原来为 1=1;
                    phmlyr.seturl = layerurl;
                    phmlyr.setsource();
                    phmlyr.Intensity = HeatMapIntensity;
                    phmlyr.Opacity = 0.9;
                    currentMap.Layers.Add(phmlyr);
                }
                return layername;
            }
            catch (Exception)
            {
                return "false";
                //throw;
            }
        }

        //LPY 2014-12-15 10:28:33 添加 热力图条件
        public string AddHeartMapLayerWithCondition(JObject jsoncommand, Map currentMap)
        {
            try
            {
                string layername = "AFD";
                string layerurl = ConfigurationManager.AppSettings[layername].ToString();
                myheatmaplayer phmlyr = _mapcontrol.getHmLyr(layername, currentMap);
                //phmlyr.Intensity = 50;
                
                //phmlyr.setfilter()
                if (phmlyr == null)
                {
                    phmlyr = new myheatmaplayer();
                    phmlyr.ID = layername;
                    phmlyr.setfilter = jsoncommand["CONDITION"].ToString().Trim(); //"1=1";  //jsoncommand["FILTER"].ToString();
                    phmlyr.seturl = layerurl;
                    phmlyr.setsource();
                    phmlyr.Intensity = HeatMapIntensity;
                    phmlyr.Opacity = 0.9;
                    currentMap.Layers.Add(phmlyr);
                }
                //phmlyr.setfilter = jsoncommand["CONDITION"].ToString().Trim();
                phmlyr.refreshnow();
                phmlyr.Refresh();
                return layername;
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /*
        public string AddHeatMapLayer(Map currentMap)
        {
            try
            {
                string layername = "AFD";
                string layerurl = ConfigurationManager.AppSettings[layername].ToString();

                myheatmaplayer phmlyr = _mapcontrol.getHmLyr(layername, currentMap);
                //phmlyr.Intensity = 50;
                if (phmlyr == null)
                {
                    phmlyr = new myheatmaplayer();
                    phmlyr.ID = layername;
                    phmlyr.setfilter = "1=1";
                    phmlyr.seturl = layerurl;
                    phmlyr.setsource();
                    phmlyr.Intensity = 30;
                    phmlyr.Opacity = 1;
                    currentMap.Layers.Add(phmlyr);
                }
                return layername;
            }
            catch (Exception)
            {
                return "false";
                //throw;
            }
        }*/

        public string DeleteHeatMapLayer(JObject jsoncommand, Map currentMap)
        {
            try
            {
                string layername = jsoncommand["LAYER"].ToString();
                myheatmaplayer phmlyr = _mapcontrol.getHmLyr(layername, currentMap);
                if (phmlyr != null)
                {
                    currentMap.Layers.Remove(phmlyr);
                    return layername;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                return "false";
                //throw;
            }
        }
        public string DeleteHeatMapLayer( Map currentMap)
        {
            try
            {
                string layername = "AFD";
                myheatmaplayer phmlyr = _mapcontrol.getHmLyr(layername, currentMap);
                if (phmlyr != null)
                {
                    currentMap.Layers.Remove(phmlyr);
                    return layername;
                }
                else
                {
                    return "false";
                }
            }
            catch (Exception)
            {
                return "false";
                //throw;
            }
        }
        #endregion
    }
}
