using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLua;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;



namespace Maverick
{
    public static class Program
    {
        //[DllImport("kernel32.dll")]
        //static extern IntPtr GetConsoleWindow();
        //[DllImport("user32.dll")]
        //static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        //const int SW_HIDE = 0;
        //const int SW_SHOW = 5;
        public static void HideConsole()
        {
            //ShowWindow(GetConsoleWindow(), SW_HIDE);
        }
        public static void ShowConsole()
        {
            //ShowWindow(GetConsoleWindow(), SW_SHOW);
        }
        static void Main(params string[] args) 
        {
            try
            {
                Console.SetWindowSize(100, 50);
                new MaverickHost(args).Run();                
            }
            catch (Exception e)
            {
                ShowConsole();
                Console.Write("Fatal exception:\n" + Utility.ExceptionToString(e));                
                Console.ReadKey();                     
            }
        }
    }
}
