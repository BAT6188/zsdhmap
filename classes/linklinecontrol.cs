using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;

namespace zsdpmap.classes
{
	public class linklinecontrol
	{
		private mapcontrol _mapcontrol = new mapcontrol();

		#region 添加关联线 
		public bool Addlinkline(MapPoint frompoint,MapPoint endpoint,GraphicsLayer linklineGlry,string linklineID)
		{
			try
			{      
				Polyline pline = new Polyline();
				ESRI.ArcGIS.Client.Geometry.PointCollection points = new ESRI.ArcGIS.Client.Geometry.PointCollection();
				points.Add(endpoint);   //edit by lzd
				points.Add(frompoint);
				pline.Paths.Add(points);
                Graphic plineg = mapcontrol.getMapG(linklineID, linklineGlry);
				if (plineg != null)
				{
					plineg.Geometry = pline;
					//return true;
				}
				else
				{
					plineg = new Graphic();
					plineg.Attributes["ID"] = linklineID;
					plineg.Geometry = pline;
					plineg.Symbol = App.Current.Resources["LinkLineSymbol"] as Symbol;
					linklineGlry.Graphics.Add(plineg);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			//	throw;
			}
		}

		public bool Addlinkline(Polyline linkline, GraphicsLayer linklineGlry, string linklineID)
		{
			try
			{
				return true;
			}
			catch (Exception)
			{
				return false;
			//	throw;
			}
		}
		#endregion

		#region 关闭关联线
		public bool Deletelinkline(GraphicsLayer linklineGlry, string linklineID)
		{
			try
			{
				Graphic plineg = mapcontrol.getMapG(linklineID, linklineGlry);
				if (plineg != null)
				{
					linklineGlry.Graphics.Remove(plineg);
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
                //throw;
			}
		}

		#endregion
	}
}
