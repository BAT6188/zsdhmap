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
using ESRI.ArcGIS.Client.Tasks.Utils.JSON;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace zsdpmap.classes
{
    public class mapcontrol
    {
        static int currentlevel = 100;   //add by lzd

        #region 切换底图

        public void changemap(JObject jsoncommand)
        {
            if (jsoncommand["M"].ToString() == "0")
            {
            }
            else
            {
            }
        }
        #endregion

        #region 根据ID得到相应的GraphicsLayer图层
        public GraphicsLayer getGlyr(string id, Map mainmap)
        {
            try
            {
                GraphicsLayer pGlyr = null;
                for (int i = 0; i < mainmap.Layers.Count; i++)
                {
                    if (mainmap.Layers[i].ID == id)
                    {
                        pGlyr = mainmap.Layers[i] as GraphicsLayer;
                        break;
                    }
                }
                return pGlyr;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        #endregion

        #region 根据ID得到相应的FeatureLayer图层
        public FeatureLayer getFlyr(string id, Map mainmap)
        {
            try
            {
                FeatureLayer pflyr = null;
                for (int i = 0; i < mainmap.Layers.Count; i++)
                {
                    if (mainmap.Layers[i].ID == id)
                    {
                        pflyr = mainmap.Layers[i] as FeatureLayer;
                        break;
                    }
                }
                return pflyr;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        #endregion

        #region 根据id查询地图GraphicsLayer对象
        public static Graphic getMapG(string id, GraphicsLayer mapGlyr)
        {
            try
            {
                Graphic pgr = null;
                foreach (Graphic gr in mapGlyr)
                {
                    if (gr.Attributes["ID"].ToString() == id)   //edit by lzd
                    {   
                        pgr = gr;
                        break;
                    };
                }
                return pgr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 根据id查询地图GraphicsLayer对象
        public static Graphic getMapGByStdid(string stdid, GraphicsLayer mapGlyr)
        {
            try
            {
                Graphic pgr = null;
                foreach (Graphic gr in mapGlyr)
                {
                    if (gr.Attributes["STDID"].ToString() == stdid)   //edit by LPY 2015-3-30 23:37:09 修改
                    {
                        pgr = gr;
                        break;
                    };
                }
                return pgr;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region 地图缩放平移功能
        //地图中心点，级数
        public void ZoomPoint(JObject jsoncommand)
        {

            try
            {
                int level = (int)jsoncommand["LEVEL"];
                
                double ratio = 1.0;
                double Resolution;

                String DELTA = ConfigurationManager.AppSettings["DELTA"].ToString();
                try
                {
                    int _delta = int.Parse(DELTA);
                    level += _delta;
 
                }
                catch (Exception)
                {
                    level += 2;
                }
                if (level > 20)         // 限制最大为20级
                    level = 20;
                
                MapPoint pPoint = new MapPoint((double)jsoncommand["CENTX"], (double)jsoncommand["CENTY"]);
                Resolution = getResolution(level);
        
      /* 老方法           
                if (currentmap.Resolution != 0.0)
                    ratio = Resolution / currentmap.Resolution;
                if (Math.Abs(1.0 - ratio) < 0.0000000001)
                    currentmap.PanTo(pPoint);
                else
                    currentmap.ZoomToResolution(Resolution, pPoint);
        */
                /*  离线切片图需要处理下列步骤
                if (level >= 13)
                {
                    GlobalLayers._MainMap.Layers["zsstreetmap"].Visible = false;
                    GlobalLayers._MainMap.Layers["zsstreetmap16"].Visible = true;
                }
                else 
                {
                    GlobalLayers._MainMap.Layers["zsstreetmap16"].Visible = false;
                    GlobalLayers._MainMap.Layers["zsstreetmap"].Visible = true;
                }
                 */
                ratio = 1.0;
                if (GlobalLayers._MainMap.Resolution != 0.0)
                    ratio = Resolution / GlobalLayers._MainMap.Resolution;

                if (Math.Abs(1.0 - ratio)< 0.0001)
                    GlobalLayers._MainMap.PanTo(pPoint);
                else
                {
                    MapPoint mapCenter = GlobalLayers._MainMap.Extent.GetCenter();
                    double X = (pPoint.X - ratio * mapCenter.X) / (1 - ratio);
                    double Y = (pPoint.Y - ratio * mapCenter.Y) / (1 - ratio);
                    // 重要 ！！ 先关闭缩放后再打开可以消除外框留白
                    //  GlobalLayers._MainMap.Layers["zsstreetmap16"].Visible = false;
                    //  GlobalLayers._MainMap.Layers["zsstreetmap"].Visible = false;

                    //LPY 2014-12-27 13:34:00 修改 YX底图的缩放显隐
                    bool flagSL = GlobalLayers._MainMap.Layers["SL"].Visible;
                    bool flagYX = GlobalLayers._MainMap.Layers["YX"].Visible;

                    if (flagSL)
                    {
                        GlobalLayers._MainMap.Layers["SL"].Visible = false;
                        GlobalLayers._MainMap.ZoomToResolution(Resolution, new MapPoint(X, Y));
                        GlobalLayers._MainMap.Layers["SL"].Visible = true;
                    }
                    if (flagYX)
                    {
                        GlobalLayers._MainMap.Layers["YX"].Visible = false;
                        GlobalLayers._MainMap.ZoomToResolution(Resolution, new MapPoint(X, Y));
                        GlobalLayers._MainMap.Layers["YX"].Visible = true;
                    }
 
                    //   GlobalLayers._MainMap.Layers["zsstreetmap"].Visible = true;
                 //   GlobalLayers._MainMap.Layers["zsstreetmap16"].Visible = true;
                }
/* 原来设计在18级以下不显示离线图层
                if (level > 13)
                {
                    
                }
                if (level > 18)                     // 离线信息 到 19级开始显示
                    GlobalLayers._offlineglr.Visible = true;
                else
                    GlobalLayers._offlineglr.Visible = false;
                */

                currentlevel = level;
   /*
                if ((currentlevel == level))     //edit by lzd
                {
                    currentmap.PanTo(pPoint);
                }
                else
                {
                    currentmap.ZoomToResolution(getResolution(level), pPoint);
                    currentmap.PanTo(pPoint);
                    currentlevel = level;
                }
    */

            }
            catch (Exception  )
            {
            //    throw e;
            }
        }


        //通过地图级别获取Resolution
        private double getResolution(int level)
        {
            /*
  switch (level)
  {
      case 0:
          return 2.0000081722216967;
      case 1:
          return 1.0000040861108483;
      case 2:
          return 0.5000020430554242;
      case 3:
          return 0.2500010215277121;
      case 4:
          return 0.12500051076385604;
      case 5:
          return 0.06250025538192802;
      case 6:
          return 0.03125012769096401;
      case 7:
          return 0.015625063845482005;
      case 8:
          return 0.007812531922741003;
      case 9:
          return 0.003906265961370448;
      case 10:
          return 0.001953132980685224;
      case 11:
          return 0.0009765664903426524;
      case 12:
          return 0.000488283245171279;
      case 13:
          return 0.00024414162258569005;
      case 14:
          return 0.00012207081129284503;
      case 15:
          return 0.00006103540564642251;
      case 16:
          return 0.00003051770282316152;
      case 17:
          return 1.525885141158076E-5;
      case 18:
          return 7.629425705840018E-6;
      case 19:
          return 3.814712852870318E-6;
      case 20:
          return 1.907351543617742E-6;
      default:
          return 0.0004568565131194138;
  }
   */
      // LV19      currentmap.Resolution = 0.0000038147128528727765
       //     TileInfo ti = GlobalLayers._MainMap.Layers[""].TileInfo;
            double ResolutionLV20 = 0.0000019073515436137569;
            return (ResolutionLV20 * Math.Pow(2,20-level));
            /*
            switch (level)
            {
                case 0:
                    return 2.0000081722219618304;
                case 1:
                    return 1.0000040861109809152;
                case 2:
                    return 0.5000020430554904576;
                case 3:
                    return 0.2500010215277452288;
                case 4:
                    return 0.1250005107638726144;
                case 5:
                    return 0.0625002553819363072;
                case 6:
                    return 0.0312501276909681536;
                case 7:
                    return 0.0156250638454840768;
                case 8:
                    return 0.0078125319227420384;
                case 9:
                    return 0.0039062659613710192;
                case 10:
                    return 0.0019531329806855096;
                case 11:
                    return 0.0009765664903427548;
                case 12:
                    return 0.0004882832451713774;
                case 13:
                    return 0.0002441416225856887;
                case 14:
                    return 0.0001220708112928443;
                case 15:
                    return 0.0000610354056464221;
                case 16:
                    return 0.0000305177028232110;
                case 17:
                    return 0.0000152588514116055;
                case 18:
                    return 0.0000076294257058027;
                case 19:
                    return 0.0000038147128529013;
                case 20:
                    return 0.0000019073564264506;
                default:
                    return 0.0000009536782132253;
            }
              */
        }

        /*
        private double getResolution(int level)
        {
            switch (level)
            {
                case 0:
                    return 0.0004568565131194138;
                case 1:
                    return 0.0002284282565597069;
                case 2:
                    return 0.00011421412827985346;
                case 3:
                    return 0.00005710706413992673;
                case 4:
                    return 0.000028553532069963364;
                case 5:
                    return 0.000014276766034981682;
                case 6:
                    return 0.000007138383017490841;
                case 7:
                    return 0.0000035691915087454205;
                default:
                    return 0.0004568565131194138;
            }
        }
         * 
         */
        #endregion

        #region GraphicsLayer图层打开
        public bool OpenGlyr(string id, Map mainmap)
        {
            try
            {
                GraphicsLayer pglyr = getGlyr(id, mainmap);
                if (!pglyr.Visible)
                {
                    pglyr.Visible = true;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
               // throw;
            }
        }
        #endregion

        #region GraphicsLayer图层关闭
        public bool CloseGlyr(string id, Map mainmap)
        {
            try
            {
                GraphicsLayer pglyr = getGlyr(id, mainmap);
                if (pglyr.Visible)
                {
                    pglyr.Visible = false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        #endregion

        #region FeatureLayer图层打开
        public bool OpenFlyr(string id, Map mainmap)
        {
            try
            {
                FeatureLayer pflyr = getFlyr(id, mainmap);
                if (!pflyr.Visible)
                {
                    pflyr.Visible = true;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        #endregion

        #region FeatureLayer图层关闭
        public bool CloseFlyr(string id, Map mainmap)
        {
            try
            {
                FeatureLayer pflyr = getFlyr(id, mainmap);
                if (pflyr.Visible)
                {
                    pflyr.Visible = false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        #endregion

        #region 根据ID得到相应的HeatMapLayer图层
        public myheatmaplayer getHmLyr(string id, Map mainmap)
        {
            try
            {
                myheatmaplayer phmlyr = null;
                for (int i = 0; i < mainmap.Layers.Count; i++)
                {
                    if (mainmap.Layers[i].ID == id)
                    {
                        phmlyr = mainmap.Layers[i] as myheatmaplayer;
                        break;
                    }
                }    
                return phmlyr;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        #endregion

    }
}
