using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;

namespace zsdpmap.classes
{
    public class crimepointcontrols
    {
        #region 在地图上添加案件点
        public bool addcrimepoint(string crimeid, double x, double y, string crimetype, string crimecontent, GraphicsLayer crimeGlyr, string symbolname)
        {
            try
            {
                Graphic pgr = new Graphic();
                pgr.Geometry = new MapPoint(x, y);
                pgr.Symbol = App.Current.Resources[symbolname] as Symbol;

                int i = App.Current.Resources.Count;

                pgr.Attributes["ID"] = crimeid;
                pgr.Attributes["name"] = crimeid;
                crimeGlyr.Graphics.Add(pgr);
                return true;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
        #endregion

            #region 在地图上删除案件点
            #endregion

            #region 在地图上修改案件点状态
            #endregion

    }
}
