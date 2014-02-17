//Copyright © 2013 Drew Rathbone.
//drewrathbone@gmail.com 
//
//This file is part of Maverick.
//
//Maverick is free software, you can redistribute it and/or modify it under the terms of GNU Affero General Public License 
//as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
//You should have received a copy of the the GNU Affero General Public License, along with Maverick. 
//
//Maverick is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
//of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
//Additional permission under the GNU Affero GPL version 3 section 7: 
//If you modify this Program, or any covered work, by linking or combining it with other code, such other code is not for 
//that reason alone subject to any of the requirements of the GNU Affero GPL version 3.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LuaInterface;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;



namespace Maverick
{
    public static class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
        public static void HideConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }
        public static void ShowConsole()
        {
            ShowWindow(GetConsoleWindow(), SW_SHOW);
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
