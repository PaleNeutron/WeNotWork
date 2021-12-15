using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeNotWork
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process[] processes = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (processes.Length > 1)
            {
                Application.Exit();
                return;
            }
            if (! Startup.IsInStartup())
            {
                Startup.RunOnStartup();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new WeNotWorkApplicationContext());
        }

        //public static void CreateStartUpShortCut()
        //{
        //    IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
        //    string shortcutAddress = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\WeNotWork.lnk";
        //    System.Reflection.Assembly curAssembly = System.Reflection.Assembly.GetExecutingAssembly();

        //    IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutAddress);
        //    shortcut.Description = "WeNotWork";
        //    shortcut.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //    shortcut.TargetPath = curAssembly.Location;
        //    shortcut.IconLocation = AppDomain.CurrentDomain.BaseDirectory + @"MyIconName.ico";
        //    shortcut.Save();
        //}

        //private static void appShortcutToDesktop(string linkName)
        //{
        //    string deskDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        //    string startupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        //    using (StreamWriter writer = new StreamWriter(startupDir + "\\" + linkName + ".url"))
        //    {
        //        string app = System.Reflection.Assembly.GetExecutingAssembly().Location;
        //        writer.WriteLine("[InternetShortcut]");
        //        writer.WriteLine("URL=file:///" + app);
        //        writer.WriteLine("IconIndex=0");
        //        string icon = app.Replace('\\', '/');
        //        writer.WriteLine("IconFile=" + icon);
        //    }
        //}
    }

   
    public class WeNotWorkApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;

        public WeNotWorkApplicationContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                ContextMenuStrip = new ContextMenuStrip(),
                Text = "We Not Work",
                Visible = true
            };
            _ = trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, onClick: Exit));

            AutoWx.Start();
        }

        void Exit(object sender, EventArgs e)
        {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            trayIcon.Visible = false;

            Application.Exit();
        }
    }
}
