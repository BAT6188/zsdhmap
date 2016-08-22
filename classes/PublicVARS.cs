using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using System.Threading;
using ESRI.ArcGIS.Client;
using System.Threading.Tasks;

namespace zsdpmap.classes
{
    class PublicVARS
    {
        public static ConcurrentDictionary<string, object> MovingTargetPool = new ConcurrentDictionary<string, object>(); // 接收到的数据池
        public static ConcurrentDictionary<string, Graphic> DrawedPool = new ConcurrentDictionary<string, Graphic>(); // 已经绘制的对象池

        public static bool ModifyFilterLock = false;

        public static Task threadtcp = null;
        public static Task threadDataudp = null;
        public static Task LoopDraw = null;
        public static Task DrawTargets = null;


        public static String MainGPS = null;
        public static String MainVID = null;
        public static double MainX = 0.0;
        public static double MainY = 0.0;
        public static double Speed = 0.0; //20.0;   // 行进速度 
        public static double Direct = 0.0;  // 行进方向
        public static double Distance = 150.0; // 50m 前置距离，使车行进前打开
        public static double CleanTime = 1800; // 预设半小时自动清理时间
        public static bool TESTMODE = false;
        public static bool GPSISLINK = false;
        public static bool AJQISLINL = false;
    }
}