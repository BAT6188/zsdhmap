using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace zsdpmap.classes
{
    static class CarFilter
    {
        static ConcurrentDictionary<string,string> Filter = new ConcurrentDictionary<string,string>();
        static bool OnlineOfflineSwitch = true; // 离在线显示开关，默认显示全部
        public static bool ModifyFilterLock = false;
        public static void SetSwitch(bool s)
        {
            OnlineOfflineSwitch = s;
        }

        public static bool GetSwitch()
        {
            return OnlineOfflineSwitch;
        }
        // 根据接收到车辆过滤表重设
        public static void ResetFilter(JArray filterlist)
        {
            try
            {
                Filter.Clear();
                foreach (Object item in filterlist)
                {
                    string v = item.ToString();
                    Filter.TryAdd(v, "1");   // 把新接收到过滤列表插入到本地过滤器
                }
            } catch (Exception)
            {

            }
        }

        public static void ClearAll()
        {
            Filter.Clear();
        }

        public static bool FindKey(string carid)
        {
            if (Filter.IsEmpty) // 没有过滤信息显示全部
                return true;
            if (Filter.ContainsKey(carid))
                return true;
            else
                return false;
        }

    }
}
