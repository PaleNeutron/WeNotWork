using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeNotWork
{
    internal class AutoWx
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); //ShowWindow needs an IntPtr

        static bool _isLocked;
        public static bool IsLocked { get => _isLocked; set => _isLocked = value; }

        public static void SessionSwitchEventHandler(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                IsLocked = false;
            }
            else if (e.Reason == SessionSwitchReason.SessionLock)
            {
                 IsLocked = true;
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        public const Int32 WM_CHAR = 0x0102;
        public const Int32 WM_KEYDOWN = 0x0100;
        public const Int32 WM_KEYUP = 0x0101;
        public const Int32 VK_RETURN = 0x0D;
        public const Int32 VK_SPACE = 0x20;

        public static void BringWxWorkWindow()
        {
            var wx_process_name = @"WXWork";
            Process[] processes = Process.GetProcessesByName(wx_process_name);

            foreach (Process p in processes)
            {
                IntPtr windowHandle = p.MainWindowHandle;
                ShowWindow(windowHandle, 9);
                SetForegroundWindow(windowHandle);

                //RECT rct = new RECT();
                //GetWindowRect(windowHandle, ref rct);
                //if (rct.Left + rct.Top > 0)
                //{
                //    LeftMouseClick(rct.Left + 80, rct.Top + 200);
                //}
                if (!IsLocked)
                {
                    SendKeys.SendWait(" ");
                } else
                {
                    PostMessage(windowHandle, WM_KEYDOWN, new IntPtr(VK_SPACE), new IntPtr(0));
                    PostMessage(windowHandle, WM_KEYUP, new IntPtr(VK_SPACE), new IntPtr(0));
                }

                break;

            }
            return;
        }

        static AutoWx()
        {
            SystemEvents.SessionSwitch += SessionSwitchEventHandler;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //This is a replacement for Cursor.Position in WinForms
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;


        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }



        public static void Start()
        {
            string startTime = "7:00 AM";
            string endTime = "11:59 PM";
            var t = new Thread(() =>
            {
                BringWxWorkWindow();
                while (true)
                {
                    var ds = DateTime.Parse(startTime);
                    var de = DateTime.Parse(endTime);
                    Thread.Sleep(1000);
                    if (DateTime.Now > ds && DateTime.Now < de && IdleTimeFinder.IdleTime() > 60 * 10)
                    {
                        BringWxWorkWindow();
                    }
                }
            });
            t.IsBackground = true;
            t.Start();
        }


    }
}
