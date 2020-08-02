using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sassafras
{
    public class TaskTrayIcon
    {
        // PROPERTIES

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;


        // CONSTRUCTORS

        public TaskTrayIcon(bool isVisible = true)
        {
            trayMenu = new ContextMenuStrip();

            Initialise();
            if (isVisible)
            {
                Show();
            }
        }


        // METHODS

        private void Initialise()
        {
            if (trayIcon != null && trayIcon.Visible) { return; }

            trayIcon = new NotifyIcon();
            Icon appIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            trayIcon.Icon = appIcon;
            trayIcon.ContextMenuStrip = trayMenu;
        }


        public void Show()
        {
            Initialise();
            trayIcon.Visible = true;
        }


        public void Close()
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
        }


        public void ShowNotification(int timeout, string title, string message, ToolTipIcon tipIcon = ToolTipIcon.None)
        {
            trayIcon.ShowBalloonTip(timeout, title, message, tipIcon);
        }


        public void AddMenuItem(string title, EventHandler onClick)
        {
            AddMenuItem(title, onClick, null);
        }


        public void AddMenuItem(string title, EventHandler onClick, Image iconImage)
        {
            trayMenu.Items.Add(title, iconImage, onClick);
            trayIcon.ContextMenuStrip = trayMenu;
        }


    }
}
