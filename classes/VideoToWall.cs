using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace zsdpmap.classes
{
    public class VideoToWall
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int InitNV(String User, String Pwd, String IP, int Port);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int InitB20(String User, String Pwd, String IP, int Port);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate uint OpenB20Win(String DB33, int x, int y, int w, int h, IntPtr mb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int CloseB20Win(uint winno, IntPtr mb);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MoveB20Win(uint winno, int x, int y, int w, int h);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int UnInitNV();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int UnInitB20();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int CloseAllB20Win();

        delegate void MsgResCallBack(IntPtr p, Int16 value, IntPtr q);

        //static IntPtr hLib;
        public struct B20ControlData
        {
            public IntPtr ptr;
            public string DB33;
            public uint winno;
        };

        public static DllInvoke dll = new DllInvoke(@"NVideo.dll");

        public static InitNV _initnv = (InitNV)dll.Invoke("InitNV", typeof(InitNV));
        public static InitB20 _initb20 = (InitB20)dll.Invoke("InitB20", typeof(InitB20));

        public static OpenB20Win _openb20win = (OpenB20Win)dll.Invoke("OpenB20Win", typeof(OpenB20Win));
        public static CloseB20Win _closeb20win = (CloseB20Win)dll.Invoke("CloseB20Win", typeof(CloseB20Win));
        public static MoveB20Win _moveb20win = (MoveB20Win)dll.Invoke("MoveB20Win", typeof(MoveB20Win));

        public static UnInitNV _uninitnv = (UnInitNV)dll.Invoke("UnInitNV", typeof(UnInitNV));
        public static UnInitB20 _uninitb20 = (UnInitB20)dll.Invoke("UnInitB20", typeof(UnInitB20));
        public static CloseAllB20Win _closeallb20win = (CloseAllB20Win)dll.Invoke("CloseAllB20Win", typeof(CloseAllB20Win));

        //[DllImport(@"NVideo.dll",EntryPoint="InitNV",CallingConvention=CallingConvention.Cdecl)]
        //public static extern int _initnv(String User, String Pwd, String IP, int Port);

        //[DllImport(@"NVideo.dll", EntryPoint = "InitB20", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int _initb20(String User, String Pwd, String IP, int Port);

        //[DllImport(@"NVideo.dll", EntryPoint = "OpenB20Win", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint _openb20win(String DB33, int x, int y, int w, int h, IntPtr mb);

        //[DllImport(@"NVideo.dll", EntryPoint = "CloseB20Win", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int _closeb20win(uint winno, IntPtr mb);

        //[DllImport(@"NVideo.dll", EntryPoint = "MoveB20Win", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int _moveb20win(uint winno, int x, int y, int w, int h);

        //[DllImport(@"NVideo.dll", EntryPoint = "UnInitNV", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int _uninitnv();

        //[DllImport(@"NVideo.dll", EntryPoint = "UnInitB20", CallingConvention = CallingConvention.Cdecl)]
        //public static extern int _uninitb20();






    }
}
