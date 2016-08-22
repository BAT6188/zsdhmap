using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using System.ComponentModel;

using ESRI.ArcGIS.Client.Local;


namespace zsdpmap.classes
{
    class QueryWithBuffer 

    {
            public event PropertyChangedEventHandler PropertyChanged;
            private bool _isBusy = true;


            public bool IsBusy
            {
                get
                {
                    return _isBusy;
                }
                set
                {
                    _isBusy = value;

                    if (PropertyChanged != null)
                    {

                        PropertyChanged(this, new PropertyChangedEventArgs("IsBusy"));
                    }
              }
            }    
    

        

        Geometry bgeom = null;
        double Distance =0.0;
        string DrawLayerName = null;
        GeometryService _geometryService;
      //   LocalService  _geometryService;

        public QueryWithBuffer(string _DrawLayerName, double _Distance) // DrawLayerName 是显示buffer结果的图层
        {
            Distance = _Distance;
            DrawLayerName = _DrawLayerName;
            _geometryService = new GeometryService(System.Configuration.ConfigurationManager.AppSettings["GeometryService"].ToString());
        //    _geometryService = new LocalGeometryService();
        }

        public void SetBuffGeom(Geometry _bgeom)
        {
            bgeom = _bgeom;
            _geometryService.BufferCompleted += GeometryService_BufferCompleted;
            _geometryService.Failed += GeometryService_Failed;
        }

        public void ProcessBuffer()   // 传入空间对象进行buffer计算，
        {

            try
            {
                _geometryService.CancelAsync();

                Graphic TempGraphic = new Graphic();
                TempGraphic.Geometry = bgeom;
                TempGraphic.Geometry.SpatialReference = new SpatialReference(4326);

                BufferParameters bufferParams = new BufferParameters()
                {
                    BufferSpatialReference = new SpatialReference(102113),
                    OutSpatialReference = new SpatialReference(4326),
                    Unit = LinearUnit.Meter
                };
         //       double degree = Distance / (106 * 1000); 
          //      bufferParams.Distances.AddRange(new double[] { degree  , degree   });
                bufferParams.Distances.Add(Distance);
                bufferParams.Features.Add(TempGraphic);
                _geometryService.BufferAsync(bufferParams);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
                           


        }


        void GeometryService_BufferCompleted(object sender, GraphicsEventArgs args)
        {

            try
            {
                GraphicsLayer _pointAndBufferGraphicsLayer;
                Geometry qGeom = null;
 
        //        GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
        //            new Action(
        //                 delegate
        //                 {
                             Graphic bufferGraphic = new Graphic();
                             bufferGraphic.Geometry = args.Results[0].Geometry;  // Buffer 的运算结果在这里，为简单化，只取第一个

                             bufferGraphic.Symbol = App.Current.Resources["BufferSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
                             bufferGraphic.SetZIndex(1);
                             qGeom = bufferGraphic.Geometry;
                             _pointAndBufferGraphicsLayer = GlobalLayers._MainMap.Layers[DrawLayerName] as GraphicsLayer;
                             
                             _pointAndBufferGraphicsLayer.Graphics.Add(bufferGraphic); // 显示Buffer结果
                             _pointAndBufferGraphicsLayer.Refresh();
          //              }
          //            )
          //      );
                // 这里默认做buffer区域里视频点的查询，查到的视频点送GQY打开
                if (qGeom != null)
                {
                    //QueryLayer _ql;//LPY 2015-1-31 09:38:00 注释掉
                    //_ql = new QueryLayer(qGeom, "CJ_SP_PT", "ResultGraphicLayer");
                    //_ql.Do();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }

        }


        private void GeometryService_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Geometry service failed: " + args.Error);
        }


    }
}

