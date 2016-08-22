using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace zsdpmap.classes
{
    class Daemon
    {
        static public void TaskMain() {
            Task task = new Task(
                 () =>
                 {
                     for (; ; )
                     {
                         if (CarFilter.FindKey("aa"))
                             continue;
                         Thread.Sleep(10000);   // 每十秒做一次扫描
                     }
                 });

        }
    }
}
