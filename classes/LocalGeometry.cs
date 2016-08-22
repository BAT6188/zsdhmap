using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client.Local;
using ESRI.ArcGIS.Client.Tasks;
using System.Configuration;
using ESRI.ArcGIS.Client;

namespace zsdpmap
{
    public partial class LocalGeometry
    {
        static GeometryService _geometryTask = null;
        static public void DistanceIsValid(Graphic G1,Graphic G2)
        {
            DistanceParameters distanceParameters = new DistanceParameters()
                {
                    DistanceUnit = LinearUnit.Meter,
                    Geodesic = true
                };
            if (_geometryTask == null)
            {
                LocalGeometryService.GetServiceAsync(lgs =>
                {
                    _geometryTask = new GeometryService();
                    _geometryTask.Url = lgs.UrlGeometryService;
                    _geometryTask.DistanceCompleted += GeometryService_DistanceCompleted;
                    _geometryTask.Failed += GeometryService_Failed;
                });
            }
            _geometryTask.DistanceAsync(G1.Geometry, G2.Geometry, distanceParameters);

        }
         static void GeometryService_DistanceCompleted(object sender, DistanceEventArgs e)
        {
            double Distance;
            try
            {
                Distance = double.Parse(ConfigurationManager.AppSettings["VALIDDISTANCE"].ToString());
            } catch (Exception)
            {
                Distance = 50;  // 默认距离 50m
            }
            if (e.Distance <= Distance)
            {
             //   CloseGQY();
            }
        }

        static private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {

        }

    }
}
