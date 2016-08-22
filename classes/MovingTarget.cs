using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Threading;
using System.Threading.Tasks;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Graphics;

namespace zsdpmap.classes
{
    class MovingTarget
    {


     //   private ConcurrentDictionary<string, object> MovingTargetPool = new ConcurrentDictionary<string, object>(); // 接收到的数据池
     //   private ConcurrentDictionary<string, Graphic> DrawedPool = new ConcurrentDictionary<string, Graphic>(); // 已经绘制的对象池



        public MovingTarget()
        {
            PublicVARS.LoopDraw = new Task(DrawProcess);
            PublicVARS.LoopDraw.Start();
        }


        public void DrawProcess()
        {
            while (true)
            {

                Parallel.ForEach(PublicVARS.MovingTargetPool, item =>
                    {
                        GlobalLayers._MainMap.Dispatcher.Invoke(
                          new Action(
                          delegate
                          {
                        Graphic Dot;
                        JObject JObj =(item.Value) as JObject;
                        if (PublicVARS.DrawedPool.TryGetValue(item.Key, out Dot)) // 查找是否已经绘制
                        {

                              // Caution! : 状态改变，RKSJ 是否也修改
                              if (JObj["RKSJ"].ToString() != Dot.Attributes["RKSJ"].ToString())  // Not a New 已经绘制改点
                              {
                                  Dot.Geometry = new MapPoint((double)JObj["X"], (double)JObj["Y"]);
                                  Dot.Attributes["angle"] = (double)JObj["DIRECT"];
                                  Dot.Attributes["RKSJ"] = JObj["RKSJ"].ToString();
                                  PublicVARS.DrawedPool.AddOrUpdate(item.Key, Dot, (K, V) => Dot);
                                  Thread.Sleep(5);
                              }

                        }
                        else
                        {   // Draw A New 

                                Dot = new Graphic();
                                Dot.Geometry = new MapPoint((double)JObj["X"], (double)JObj["Y"]);
                                Dot.Attributes["angle"] = (double)JObj["DIRECT"];
                                Dot.Attributes["RKSJ"] = JObj["RKSJ"].ToString();
                                Dot.Attributes["num"] = JObj["TITLE"].ToString(); 
                                Dot.Symbol = App.Current.Resources["policecar0"] as Symbol;
                                GlobalLayers._policepointglr.Graphics.Add(Dot);
                                PublicVARS.DrawedPool.TryAdd(item.Key, Dot);
                                Thread.Sleep(5);

                        }
                          }));
                    });

                Thread.Sleep(10);
            }

        }

        public void AddToHashTable(JObject MovingData)
        {
            if (MovingData["CMD"].ToString() == "00080")
            {
                PublicVARS.MovingTargetPool.AddOrUpdate(MovingData["HH"].ToString(), MovingData, (K, V) => MovingData);

                string policetype = MovingData["TYPE"].ToString().Trim();   // 取出动态点数据的类型
                if (PublicVARS.MainGPS  == MovingData["HH"].ToString())
                {
                    PublicVARS.MainX = (double)MovingData["X"];
                    PublicVARS.MainY = (double)MovingData["Y"];
                    PublicVARS.Speed = (double)MovingData["SPEED"];
                    PublicVARS.Direct = (double)MovingData["DIRECT"];
                }

            }

        }
    }
}
