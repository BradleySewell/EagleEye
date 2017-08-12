using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace EagleEye.Entities.Wrappers
{
    public static class User32Wrapper
    {
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private extern static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);



        public static void MinimizeWindow(IntPtr hwnd)
        {
            try
            {
                ShowWindow(hwnd, SW_MINIMIZE);
            }
            catch (Exception e)
            { }
        }

        public static void MaximizeWindow(IntPtr hwnd)
        {
            try
            {
                ShowWindow(hwnd, SW_MAXIMIZE);
            }
            catch (Exception e)
            { }
        }

        public static void ExecuteCommand(string command, bool hidden)
        {
            using (Process proc = new Process())
            {
                string cmdPath = "C:\\windows\\system32\\cmd.exe";
                if (File.Exists(cmdPath))
                {
                    proc.StartInfo.FileName = cmdPath;
                    if (hidden)
                    {
                        proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    }
                    proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(cmdPath);
                    proc.StartInfo.Arguments = "/c " + command;
                    proc.Start();
                }
            }
        }

        public static void ExecuteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                {
                    proc.StartInfo.FileName = filePath;
                    proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(filePath);
                    proc.Start();
                }
            }
        }

    }
}
