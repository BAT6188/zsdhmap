using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Client;

namespace zsdpmap.classes
{
    static class Util
    {
        public static void SetSwitchOfflineLayar(bool s)
        {
            try
            {
                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                    delegate
                    {
                        if (s)
                            GlobalLayers._offlineglr.Visible = false;
                        else
                            GlobalLayers._offlineglr.Visible = true;
                    }));
            }
            catch (Exception)
            {

            }

        }
        public static bool ClearLayer(GraphicsLayer _layer)
        {
            try
            {
                GlobalLayers._MainMap.Dispatcher.Invoke(              // 此结构是WIN GUI 处理多线程修改界面元素的保护
                    new Action(
                    delegate
                    {
                        _layer.Graphics.Clear();
                    }));
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
