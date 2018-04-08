using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MultiAdmin.MultiAdmin;

namespace MutliAdmin
{
    public static class Program
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            //ShowWindow(handle, 0);
            Application.Run(new GUI(args)); // WinForm Instance
        }
    }
}