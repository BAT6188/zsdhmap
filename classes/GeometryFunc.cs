using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Tasks.Utils.JSON;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;


namespace zsdpmap.classes
{
    class GeometryFunc
    {
        public static int GetZoomLevel()
        {
            double currentRes = GlobalLayers._MainMap.Resolution;
          //  ArcGISLocalTiledLayer basetile = (ArcGISLocalTiledLayer)GlobalLayers._MainMap.Layers[0];
            MyTile basetile = (MyTile)GlobalLayers._MainMap.Layers[0];
            var lods = basetile.TileInfo.Lods.ToList();
            int level = 20 ,i;
            for (i = 0; i < lods.Count(); i++)
            {
                if (Math.Abs(currentRes - lods[i].Resolution) < 0.000001)
                {
                    level = i;
                    break;
                }
            }
                // int level = lods.IndexOf(lods.Where(l => l.Resolution == currentRes).FirstOrDefault());

                return level;
        }
///        此方法为ArcGIS for silverlight 中根据已知点生成圆状polygon（实为点集）
///
/// 360度画圆
///
/// 半径Wgs-84坐标系下1度约等于 111194.872221777米
/// 中心点即为需要生成圆的基本点

/// 返回Graphic 
        public static Geometry GetEllipseGraphic(double radius, MapPoint centerP)
        {

            ESRI.ArcGIS.Client.Geometry.PointCollection pCollection = new ESRI.ArcGIS.Client.Geometry.PointCollection();
            for (double i = 0; i <= 360; i += 1)
            {
                pCollection.Add(new MapPoint((centerP.X - Math.Cos(Math.PI * i / 180.0) * radius), (centerP.Y - Math.Sin(Math.PI * i / 180.0) * radius)));
            }

            ESRI.ArcGIS.Client.Geometry.Polygon g = new ESRI.ArcGIS.Client.Geometry.Polygon();
            g.Rings.Add(pCollection);

            return g;
            //       result.Symbol = this.LayoutRoot.Resources["DefaulFillSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;//这里根据自己的需要定义样式

        }

        /// <summary>
        /// 将返回的json解析为Geometry,不考虑坐标包含M和Z，如果考虑，请改动即可。将ArcObjects的Geometry转为json的代码我正在测试。
        /// 作者：刘宇
        /// 时间2012年
        /// </summary>
        /// <param name="jsonResponse"></param>
        /// <returns></returns>

        public static Geometry ParsefromJson(JObject jsonObject)
        {
            
            try
            {
             //   JObject jsonObject = JObject.Parse(jsonResponse) as JObject;
                
                SpatialReference pSpatial = new SpatialReference();
                Geometry pGeo = null;

                if (jsonObject.Property("geometries") != null || jsonObject.Property("geometry") != null)
                {
               //     JArray jsonGArray = jsonObject["geometries"] as JArray;
                    JArray jsonGArray;
                    JObject jsonObjectGeo;

                    if (jsonObject.Property("geometry") != null)
                        jsonObjectGeo = jsonObject["geometry"] as JObject;
                    else
                    {
                        jsonGArray = jsonObject["geometries"] as JArray;
                        if (jsonGArray.Count() < 1)     // 只返回一个GEOM 对象
                        {
                            return null;
                        }

                        jsonObjectGeo = jsonGArray[0] as JObject;
                    }

                    
                    //空间参考信息
                    if (jsonObjectGeo.Property("spatialReference") != null)
                    {
                        pSpatial = new SpatialReference(4326);


                        //JsonObject pSpatialJson =jsonObjectGeo["spatialReference"] as JsonObject;

                        //   根据需要添加                 
                    }
                    //点线面对象，不考虑hasz和hasM
                    if (jsonObjectGeo.Property("points") != null)
                    {
                        JArray JsonPoints = jsonObjectGeo["points"] as JArray;

                        if (JsonPoints is JArray)
                        {
                            if (JsonPoints.Count() == 1)
                            {
                                MapPoint pPoint = new MapPoint();

                                //去掉中括号

                                string[] pStrPoints = JsonPoints[0].ToString().Substring(1, JsonPoints[0].ToString().Length - 2).Split(',');

                                pPoint.X = Convert.ToDouble(pStrPoints[0]);
                                pPoint.Y = Convert.ToDouble(pStrPoints[1]);

                                pGeo = pPoint;


                            }

                        }
                    }
                    else if (jsonObjectGeo.Property("paths") != null)
                    {
                        JArray JsonPoints = jsonObjectGeo["paths"] as JArray;

                        ESRI.ArcGIS.Client.Geometry.Polyline pPolyline = new ESRI.ArcGIS.Client.Geometry.Polyline();


                        ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection> pPointCollection = new ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection>();
                        // pPolyline.Paths

                        if (JsonPoints is JArray)
                        {
                            for (int i = 0; i < JsonPoints.Count(); i++)
                            {
                                if (JsonPoints[i] is JArray)
                                {
                                    ESRI.ArcGIS.Client.Geometry.PointCollection pPointCollections = new ESRI.ArcGIS.Client.Geometry.PointCollection();

                                    JArray pInnerPoints = JsonPoints[i] as JArray;
                                    for (int j = 0; j < pInnerPoints.Count; j++)
                                    {
                                        string pStr = pInnerPoints[j].ToString();

                                        string[] pStrPoints = pInnerPoints[j].ToString().Substring(1, pInnerPoints[j].ToString().Length - 2).Split(',');
                                        MapPoint pPoint = new MapPoint();
                                        pPoint.X = Convert.ToDouble(pStrPoints[0]);
                                        pPoint.Y = Convert.ToDouble(pStrPoints[1]);

                                        pPointCollections.Add(pPoint);
                                    }

                                    pPointCollection.Add(pPointCollections);

                                }
                            }

                            pPolyline.Paths = pPointCollection;

                            pGeo = pPolyline;
                        }
                    }
                    else if (jsonObjectGeo.Property("rings") != null)
                    {
                        JArray JsonPoints = jsonObjectGeo["rings"] as JArray;

                        ESRI.ArcGIS.Client.Geometry.Polygon pPolygon = new ESRI.ArcGIS.Client.Geometry.Polygon();



                        ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection> pPointCollection = new ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection>();


                        if (JsonPoints is JArray)
                        {
                            for (int i = 0; i < JsonPoints.Count(); i++)
                            {
                                if (JsonPoints[i] is JArray)
                                {
                                    ESRI.ArcGIS.Client.Geometry.PointCollection pPointCollections = new ESRI.ArcGIS.Client.Geometry.PointCollection();

                                    JArray pInnerPoints = JsonPoints[i] as JArray;
                                    for (int j = 0; j < pInnerPoints.Count; j++)
                                    {
                                        string pStr = pInnerPoints[j].ToString();

                                        string[] pStrPoints = pInnerPoints[j].ToString().Substring(1, pInnerPoints[j].ToString().Length - 2).Split(',');
                                        MapPoint pPoint = new MapPoint();
                                        pPoint.X = Convert.ToDouble(pStrPoints[0]);
                                        pPoint.Y = Convert.ToDouble(pStrPoints[1]);

                                        pPointCollections.Add(pPoint);
                                    }

                                    pPointCollection.Add(pPointCollections);

                                }
                            }

                            pPolygon.Rings = pPointCollection;

                            pGeo = pPolygon;

                        }
                    }
                }


                if (pGeo != null)
                    pGeo.SpatialReference = pSpatial;

                return pGeo;
            }
            catch (Exception)   // 解析出现异常返回空
            {
                return null;
            }
        }
    }
}
