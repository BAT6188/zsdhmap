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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace zsdpmap.classes
{
    class GeomProc
    {
        /*     public static void GeomDrawAndSearch(JObject receiveGeomStr)
             {
                 Geometry geom = GeometryFunc.ParsefromJson(receiveGeomStr);
                 if (geom == null)
                     return;
                 Graphic g = new Graphic();
                 g.Geometry = geom;

                 QueryTask queryTask = new QueryTask();
                 queryTask.Url = _mapService.UrlMapService + "/2";
                 queryTask.ExecuteCompleted += QueryTask_ExecuteCompleted;
                 queryTask.Failed += QueryTask_Failed;
                 Binding resultFeaturesBinding = new Binding("LastResult.Features");
                 resultFeaturesBinding.Source = queryTask;
                 QueryDetailsDataGrid.SetBinding(DataGrid.ItemsSourceProperty, resultFeaturesBinding);

                 Query query = new ESRI.ArcGIS.Client.Tasks.Query();
                 query.OutFields.AddRange(new string[] { "STATE_NAME", "SUB_REGION", "STATE_FIPS", "STATE_ABBR", "POP2000", "POP2010" });
                 query.Geometry = geom;
                 query.ReturnGeometry = true;
                 queryTask.ExecuteAsync(query);

                 GlobalLayers._MainMap.Dispatcher.Invoke(
                     new Action(
                         delegate
                         {
                             GlobalLayers.canvasLayer.Graphics.Add(g);
                         }
                     )
                 );

        
             }
         * */
        public static void ClearCanvas()
        {
            GlobalLayers._MainMap.Dispatcher.Invoke(
                new Action(
                    delegate
                    {
                        GlobalLayers.canvasLayer.ClearGraphics();
                    }
                )
            );

        }

        private void QueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;
/*
            if (featureSet == null || featureSet.Features.Count < 1)
            {
           //     MessageBox.Show("No features returned from query");
                return;
            }

            GraphicsLayer graphicsLayer = GlobalLayers._MainMap.Layers["canvas"] as GraphicsLayer;
            if (featureSet != null && featureSet.Features.Count > 0)
            {
                foreach (Graphic feature in featureSet.Features)
                {
                    feature.Symbol = LayoutRoot.Resources["ResultsFillSymbol"] as FillSymbol;
                    graphicsLayer.Graphics.Insert(0, feature);
                }
                ResultsDisplay.Visibility = Visibility.Visible;
            }
            MyDrawObject.IsEnabled = false;
 * */
        }

        private void QueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
       //     MessageBox.Show("Query failed: " + args.Error);
        }


    }
}
