using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace zsdpmap.classes
{
    public class DllInvoke
    {
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(String path);
        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);
        [DllImport("kernel32.dll")]
        private extern static bool FreeLibrary(IntPtr lib);
        delegate void MsgResCallBack(IntPtr p, Int16 value, IntPtr q);

        private static IntPtr hLib;// = LoadLibrary(@"NVideo.dll");

        public DllInvoke(string DLLPath)
        {
            try
            {
                hLib = LoadLibrary(DLLPath);
            }
            catch (Exception)
            {
                
            }
            
        }

        ~DllInvoke()
        {
            FreeLibrary(hLib);
        }

        public Delegate Invoke(String APIName, Type t)
        {
            IntPtr api = GetProcAddress(hLib, APIName);
            if (api == null)
                return null;
            else
                return (Delegate)Marshal.GetDelegateForFunctionPointer(api, t);
        }
    }
}
