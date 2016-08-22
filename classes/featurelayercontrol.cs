using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Graphics;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace zsdpmap.classes
{
	public class featurelayercontrol
	{
		mapcontrol _mapcontrol = new mapcontrol();

		#region FeatureLayer Add&Remove
		public string AddFeaturelayer(JObject jsoncommand,Map currentMap)
		{
			try
			{
				string layername = jsoncommand["LAYER"].ToString();
				string layerurl = System.Configuration.ConfigurationManager.AppSettings[layername].ToString();
				FeatureLayer pfealyr = _mapcontrol.getFlyr(layername, currentMap);
				if (pfealyr == null)
				{
					pfealyr = new FeatureLayer();
					pfealyr.Url = layerurl;
					pfealyr.ID = layername;
					pfealyr.Where = jsoncommand["FILTER"].ToString();
                    if (layername == "video" || layername == "CJ_SP_PT")
                    {
                        pfealyr.Where = " 1=1 ";//"QYDM like '330902%'";
                        SimpleRenderer render = new SimpleRenderer();
                        render.Symbol = (Symbol)App.Current.Resources[layername];
                        pfealyr.Renderer = render;
                    }
                    else
                    {
                        if (layername == "XQ_SQ_CJ_PT" || layername == "FQ_GAFJ_PG" || layername == "FQ_JWXQ_PCS_PG" || layername == "XQ_ZRQ_CJ_PG")//2015-1-31 12:19:48 有些面图层不需加载符号
                        {
                            if (layername == "XQ_SQ_CJ_PT" || layername == "XQ_ZRQ_CJ_PG")//社区图层半透明
                            {
                                pfealyr.Opacity = 0.5;
                            }
                        }
                        else
                        {
                            SimpleRenderer render = new SimpleRenderer();
                            render.Symbol = (Symbol)App.Current.Resources["SYB_" + layername];
                            pfealyr.Renderer = render;
                        }
                    }
					pfealyr.Visible = true;
					currentMap.Layers.Add(pfealyr);

					return layername;
				}
				else
				{
					//pfealyr.Visible=true;
					return "false";
				}
			}
			catch (Exception)
			{
				return "false";
				//throw;
			}
		}

		public string DeleteFeaturelayer(JObject jsoncommand,Map currentMap)
		{
			try 
			{	        
				string layername = jsoncommand["LAYER"].ToString();
				FeatureLayer pfealyr = _mapcontrol.getFlyr(layername, currentMap);
				if (pfealyr != null)
				{
					currentMap.Layers.Remove(pfealyr);
					return layername;
				}else
				{
					 return "false";
				}
			}
			catch (Exception)
			{
				return "false";
				//throw;
			}
		}

		#endregion
	}
}
