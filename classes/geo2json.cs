using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.Tasks.Utils.JSON;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace zsdpmap.classes
{
    class geo2json
    {







        ///刘宇

        private Geometry ParsefromJson(string jsonResponse)
        {

            JsonObject jsonObject = JObject.Parse(jsonResponse.ToString()) as JObject;
            SpatialReference pSpatial = new SpatialReference();
            Geometry pGeo = null;

            if (jsonObject.Contains("","")) //    .ContainsKey("geometries"))
            {


                JObject jsonObjectGeo = jsonObject["geometries"] as JObject;
                //空间参考信息
          /*      if (jsonObjectGeo.ContainsKey("spatialReference"))
                {
                    pSpatial = this.myMap.SpatialReference;


                    //JsonObject pSpatialJson =jsonObjectGeo["spatialReference"] as JsonObject;

                    //   if(pSpatialJson.ContainsKey("wkid"))
                    //   {
                    //       pSpatial.WKID = Convert.ToInt16(pSpatialJson["wkid"].ToString());
                    //   }
                    //   else if(pSpatialJson.ContainsKey("wkt"))
                    //   {
                    //       pSpatial.WKT = pSpatialJson["wkt"].ToString();
                    //   }




                }
           * */
                //点线面对象，不考虑hasz和hasM
                if (jsonObjectGeo.ContainsKey("points"))
                {
                    JValue JsonPoints = jsonObjectGeo["points"];

                    if (JsonPoints is JsonArray)
                    {
                        if (JsonPoints.Count == 1)
                        {
                            MapPoint pPoint = new MapPoint();

                            //去掉中括号

                            string[] pStrPoints = JsonPoints[0].ToString().Substring(1, JsonPoints[0].ToString().Length - 2).Split(',');

                            pPoint.X = Convert.ToDouble(pStrPoints[0]);
                            pPoint.Y = Convert.ToDouble(pStrPoints[1]);

                            pGeo = pPoint;


                        }
                        //else
                        //{
                        //    ESRI.ArcGIS.Client.Geometry.PointCollection pPointCollection = new ESRI.ArcGIS.Client.Geometry.PointCollection();
                        //    for (int i = 0; i < JsonPoints.Count; i++)
                        //    {
                        //        string pStr = JsonPoints[i].ToString();
                        //        MapPoint pPoint = new MapPoint();
                        //        string[] pStrPoints = JsonPoints[0].ToString().Substring(1, JsonPoints[0].ToString().Length - 2).Split(',');
                        //        pPoint.X = Convert.ToDouble(pStrPoints[0]);
                        //        pPoint.Y = Convert.ToDouble(pStrPoints[1]);
                        //        pPointCollection.Add(pPoint);
                        //    }

                        //    pGeo = pPointCollection as ESRI.ArcGIS.Client.Geometry.Geometry;
                        //}


                    }
                }
                else if (jsonObjectGeo.ContainsKey("paths"))
                {
                    JsonValue JsonPoints = jsonObjectGeo["paths"];

                    ESRI.ArcGIS.Client.Geometry.Polyline pPolyline = new ESRI.ArcGIS.Client.Geometry.Polyline();


                    ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection> pPointCollection = new ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection>();
                    // pPolyline.Paths

                    if (JsonPoints is JsonArray)
                    {
                        for (int i = 0; i < JsonPoints.Count; i++)
                        {
                            if (JsonPoints[i] is JsonArray)
                            {
                                ESRI.ArcGIS.Client.Geometry.PointCollection pPointCollections = new ESRI.ArcGIS.Client.Geometry.PointCollection();

                                JsonArray pInnerPoints = JsonPoints[i] as JsonArray;
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
                else if (jsonObjectGeo.ContainsKey("rings"))
                {
                    JsonValue JsonPoints = jsonObjectGeo["rings"];

                    ESRI.ArcGIS.Client.Geometry.Polygon pPolygon = new ESRI.ArcGIS.Client.Geometry.Polygon();



                    ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection> pPointCollection = new ObservableCollection<ESRI.ArcGIS.Client.Geometry.PointCollection>();


                    if (JsonPoints is JsonArray)
                    {
                        for (int i = 0; i < JsonPoints.Count; i++)
                        {
                            if (JsonPoints[i] is JsonArray)
                            {
                                ESRI.ArcGIS.Client.Geometry.PointCollection pPointCollections = new ESRI.ArcGIS.Client.Geometry.PointCollection();

                                JsonArray pInnerPoints = JsonPoints[i] as JsonArray;
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



            pGeo.SpatialReference = pSpatial;

            return pGeo;
        }



        ///刘宇

        private string Geometry2Json(IGeometry pGeo)
        {

            int wkid = pGeo.SpatialReference.FactoryCode;
            ESRI.ArcGIS.Geometry.IPoint pPoint = null;
            ESRI.ArcGIS.Geometry.IPointCollection pPoints = null;
            double x, y;
            StringBuilder sb = new StringBuilder("{");
            sb.Append(@"""geometries""" + ":{");

            switch (pGeo.GeometryType)
            {
                #region Point2Json
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint:
                    pPoint = pGeo as ESRI.ArcGIS.Geometry.IPoint;
                    pPoint.QueryCoords(out x, out y);
                    string json = @"{""x"":" + x + @",""y"":" + y + @",""spatialReference"":" + @"{""wkid"":" + wkid + "}";
                    sb.Append(@"""point"":" + json);

                    break;
                #endregion

                #region Polyline2Json
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline:
                    pPoints = pGeo as ESRI.ArcGIS.Geometry.IPointCollection;

                    IPolyline pPolyline = pGeo as IPolyline;

                    IGeometryCollection pGeoetryCollection = pPolyline as IGeometryCollection;

                    if (pGeoetryCollection.GeometryCount >= 1)
                    {
                        sb.Append(@"""paths"":[");
                        for (int i = 0; i < pGeoetryCollection.GeometryCount; i++)
                        {
                            //paths可能有多个path，而每一个path是多个点，用两个for循环
                            if (pGeoetryCollection.get_Geometry(i) is IPath)
                            {
                                sb.Append("[");
                                pPoints = pGeoetryCollection.get_Geometry(i) as IPointCollection;

                                for (int j = 0; j < pPoints.PointCount; j++)
                                {
                                    pPoint = pPoints.get_Point(j);
                                    pPoint.QueryCoords(out x, out y);
                                    sb.Append("[" + x + "," + y + "],");
                                }
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append("]");
                            }
                        }
                        sb.Append("]" + @",""spatialReference"":" + @"{""wkid"":" + wkid + "}");

                    }
                    //else
                    //{
                    //     sb.Append(@"""paths"":[[");
                    //for (int i = 0; i < pPoints.PointCount; i++)
                    //{
                    //    pPoint = pPoints.get_Point(i);
                    //    pPoint.QueryCoords(out x, out y);
                    //    sb.Append("[" + x + "," + y + "],");
                    //}
                    //sb.Remove(sb.Length - 1, 1);
                    //sb.Append("]]" + @",""spatialReference"":" + @"{""wkid"":" + wkid + "}");
                    //}


                    break;

                #endregion

                #region Polygon2Json
                case ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon:
                    pPoints = pGeo as ESRI.ArcGIS.Geometry.IPointCollection;



                    IPolygon pPolygon = pGeo as IPolygon;

                    //外环和内环？面的构造比较复杂



                    IGeometryCollection pGeoetryCollection1 = pPolygon as IGeometryCollection;

                    if (pGeoetryCollection1.GeometryCount >= 1)
                    {
                        sb.Append(@"""rings"":[");
                        for (int i = 0; i < pGeoetryCollection1.GeometryCount; i++)
                        {

                            if (pGeoetryCollection1.get_Geometry(i) is IRing)
                            {
                                sb.Append("[");
                                pPoints = pGeoetryCollection1.get_Geometry(i) as IPointCollection;
                                for (int j = 0; j < pPoints.PointCount; j++)
                                {

                                    pPoint = pPoints.get_Point(j);
                                    pPoint.QueryCoords(out x, out y);
                                    sb.Append("[" + x + "," + y + "],");
                                }

                                sb.Remove(sb.Length - 1, 1);
                                sb.Append("]");
                            }
                        }
                        sb.Append("]" + @",""spatialReference"":" + @"{""wkid"":" + wkid + "}");

                    }
                    //else
                    //{
                    //    sb.Append(@"""rings"":[[");
                    //    for (int i = 0; i < pPoints.PointCount; i++)
                    //    {
                    //        pPoint = pPoints.get_Point(i);
                    //        pPoint.QueryCoords(out x, out y);
                    //        sb.Append("[" + x + "," + y + "],");
                    //    }
                    //    sb.Remove(sb.Length - 1, 1);
                    //    sb.Append("]]" + @",""spatialReference"":" + @"{""wkid"":" + wkid + "}");
                    //}


                    break;
                #endregion
            }

            sb.Append("}");

            //添加Geometry

            sb.Append("}");

            return sb.ToString();

        }



    }
}
