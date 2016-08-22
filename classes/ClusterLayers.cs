using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Local;
using ESRI.ArcGIS.Client.Clustering;

using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Tasks.Utils.JSON;
using ESRI.ArcGIS.Client.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Windows.Media;

namespace zsdpmap.classes
{
    /// <summary>
    /// LPY 2015-8-11 15:24:54 添加
    /// 聚合图相关操作类
    /// 该类之前已经存在，但无任何内容。所有内容均为新增
    /// </summary>
    public class ClusterLayers
    {
        private const string CLUSTERLAYERID = "clusterLayer";

        public static bool AddClusterLayerByCondition(JObject jsonCommand,Map currentMap)
        {
            try
            {
                if (jsonCommand["CONDITION"]==null||jsonCommand["CONDITION"].ToString()=="")//判断条件
                {
                    return false;
                }
                string urlName = "AFD";
                string url = ConfigurationManager.AppSettings[urlName].ToString();

                SimpleRenderer sr = new SimpleRenderer();
                sr.Symbol = App.Current.Resources["ClusterMarkerSymbol"] as Symbol;//默认symbol  MyClassBreaksRendererPoints   ClusterMarkerSymbol

                //FlareClusterer fc = new FlareClusterer()//设置聚合条件
                //{
                //    FlareForeground = new SolidColorBrush(Colors.Black),
                //    FlareBackground = new SolidColorBrush(Colors.Yellow),
                //    MaximumFlareCount = 15,
                //    Radius = 40,
                //    Gradient = App.Current.Resources["BlueGradient"] as LinearGradientBrush
                //};

                FeatureLayer clusterLayer = new FeatureLayer()
                {
                    Url = url,
                    Renderer = sr,
                    ID = CLUSTERLAYERID,
                    Where = jsonCommand["CONDITION"].ToString(),
                    //Clusterer = fc,
                   
                };

                //LPY 2016-4-14 添加 更改大屏聚合图层的基本样式，扩大字体
                SumClusterer sumClusterer = new SumClusterer() { Radius = 80 };
                clusterLayer.Clusterer = sumClusterer;

                currentMap.Layers.Add(clusterLayer);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据图层名称删除
        /// </summary>
        /// <param name="clusterLayerName"></param>
        public static void DeleteClusterLayerByLayerName()
        {
            try
            {
                string clusterLayerID = CLUSTERLAYERID;
                for (int i = 0; i < GlobalLayers._MainMap.Layers.Count; i++)
                {
                    if (GlobalLayers._MainMap.Layers[i].ID==clusterLayerID)
                    {
                        GlobalLayers._MainMap.Layers.RemoveAt(i);
                        break;
                    }
                }
            }
            catch (Exception)
            {            }            
        }

        #region 查找聚合图层
        public static FeatureLayer getClusterLayer(string mapId, Map currentMap)
        {
            try
            {
                FeatureLayer fl = null;
                for (int i = 0; i < currentMap.Layers.Count; i++)
                {
                    if (currentMap.Layers[i].ID==CLUSTERLAYERID)
                    {
                        fl = (FeatureLayer)currentMap.Layers[i];
                        break;
                    }                    
                }
                return fl;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

    }
}
