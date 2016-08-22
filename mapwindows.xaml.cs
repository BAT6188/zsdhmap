using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;

namespace zsdpmap
{
    /// <summary>
    /// Interaction logic for mapwindows.xaml
    /// </summary>
    public partial class mapwindows : UserControl
    {
        public mapwindows()
        {
            InitializeComponent();
        }

        private void map_Loaded(object sender, RoutedEventArgs e)
        {
            //map.Extent = new Envelope(122.228531, 30.022, 122.229531, 30.032);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //System.Threading.Thread.Sleep(2000);
        }

        private void ArcGISLocalTiledLayer_TileLoaded(object sender, TiledLayer.TileLoadEventArgs e)
        {
            //MapPoint pPoint = new MapPoint(122.228531, 30.022);
            //map.ZoomToResolution(0.0000035691915087454205, pPoint);
            //MessageBox.Show(map.Resolution.ToString()+"local");
        }
    }
}
