using EagleEye.Entities.DataContext;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Application = System.Windows.Application;

namespace EagleEye.Startup
{
    public class NotifyIconHandler
    {
        private readonly AppDataContext _appDataContext;
        private readonly System.Windows.Forms.NotifyIcon _notifyIcon;
        private readonly System.Drawing.Icon _standardIcon;
        private readonly System.Drawing.Icon _workingIcon;

        public delegate void OnClick();
        public event OnClick OnClickEvent;

        public NotifyIconHandler(AppDataContext appDataContext, System.Drawing.Icon standardIcon, System.Drawing.Icon workingIcon)
        {
            _appDataContext = appDataContext;
            _standardIcon = standardIcon;
            _workingIcon = workingIcon;

            _notifyIcon = new System.Windows.Forms.NotifyIcon { BalloonTipText = "EagleEye", Icon = standardIcon, Visible = true };

            var menu = new System.Windows.Forms.ContextMenu();
            menu.MenuItems.Add("Show", delegate (object sender, EventArgs e)
            {
                NotifyDoubleClick(sender, null);
            });
            menu.MenuItems.Add("Clear", delegate (object sender, EventArgs e)
            {
                _appDataContext.StrokeCollection.Clear();
                _appDataContext.VideoSelectedImage = null;
                _appDataContext.ImageDifference = null;
                _appDataContext.ImageChangePercentage = 0.0;
                _appDataContext.Status = string.Empty;
            });
            menu.MenuItems.Add("Exit", delegate (object sender, EventArgs e)
            {
                Application.Current.Shutdown();
            });

            _notifyIcon.ContextMenu = menu;

            _notifyIcon.MouseDoubleClick += (sender, e) => NotifyDoubleClick(sender, e);
            _notifyIcon.BalloonTipClicked += (sender, e) => BalloonTipClick(sender, e);

            ShowAlert("EagleEye is running...", TimeSpan.FromSeconds(5));

            AutoShowInSystemTray();
        }

        private void NotifyDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnClickEvent();
        }

        private void BalloonTipClick(object sender, EventArgs e)
        {
            OnClickEvent();
        }

        private void AutoShowInSystemTray()
        {
            try
            {
                var registryKey = Registry.CurrentUser.CreateSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\TrayNotify");
                registryKey.SetValue("EnableAutoTray", 0);
            }
            catch (Exception e)
            {
            }
        }


        public void ShowAlert(string message, TimeSpan timeSpan)
        {
            _notifyIcon.ShowBalloonTip(Convert.ToInt32(timeSpan.TotalMilliseconds), "", message, System.Windows.Forms.ToolTipIcon.None);
        }

        public void SetStandardIcon()
        {
            _notifyIcon.Icon = _standardIcon;
        }

        public void SetWorkingIcon()
        {
            _notifyIcon.Icon = _workingIcon;
        }
    }
}
