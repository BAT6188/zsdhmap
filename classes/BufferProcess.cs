using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Local;

using System.Windows.Controls;

namespace zsdpmap.classes
{
    class BufferProcess
    {
        GeometryService MyGS;
        BufferParameters _BufferParameters;
        IList<Graphic> results;
        Boolean Completed;
        Boolean Failed;
        Grid LayoutRoot;
        GraphicsLayer graphicsLayer;

        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {
            results = args.Results;
            Completed = true;
       //     GraphicsLayer graphicsLayer = MyMap.Layers["MyGraphicsLayer"] as GraphicsLayer;

 
        }

        private void GeometryService_Failed(object sender, TaskFailedEventArgs e)
        {
            Failed = true;
        }
        public BufferProcess(GraphicsLayer _graphicsLayer,Grid _LayoutRoot)
        {
            graphicsLayer = _graphicsLayer;
            LayoutRoot = _LayoutRoot;
            /*
             * 
            MyGS = new GeometryService("http://tasks.arcgisonline.com/ArcGIS/rest/services/Geometry/GeometryServer");
            BufferParameters _BufferParameters = new BufferParameters();
            _BufferParameters.BufferSpatialReference = new SpatialReference(4326);
            _BufferParameters.Distances.Add(100);
            _BufferParameters.Unit = LinearUnit.Meter;
            MyGS.BufferCompleted += GeometryService_BufferCompleted;
            MyGS.Failed += GeometryService_Failed;
            Completed = false;
            Failed = false;
            */
            LocalGeometryService.GetServiceAsync(LocalGeometryService =>
                {
                    MyGS = new GeometryService();
                    MyGS.Url = LocalGeometryService.UrlGeometryService;
                    MyGS.BufferCompleted += GeometryService_BufferCompleted;
                    MyGS.Failed += GeometryService_Failed;

                }
                );
            Completed = false;
            Failed = false;
        }

        public int _Action(Graphic _G)
        {
            _BufferParameters.Features.Add(_G);
            MyGS.BufferAsync(_BufferParameters);
            while (!Failed && !Completed)
            {
                foreach (Graphic graphic in results)
                {
                    graphic.Symbol = LayoutRoot.Resources["DefaultBufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                    graphicsLayer.Graphics.Add(graphic);
                }
            }
            return 0;

        }
    }
}
