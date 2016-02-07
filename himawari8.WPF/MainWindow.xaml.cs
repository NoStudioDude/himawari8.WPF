using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

namespace himawari8.WPF
{
    public partial class MainWindow : Window
    {

        private HimawariViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new HimawariViewModel();
            SetTray();
        }

        private void SetTray()
        {

            NotifyIcon trayIcon = new NotifyIcon();
            trayIcon.Text = "Himawari8";
            trayIcon.Icon = Properties.Resources.icon;
            
            ContextMenu trayMenu = new ContextMenu();
            MenuItem menuItemApplication = new MenuItem("Application");
            MenuItem menuItemSettings = new MenuItem("Settings");
            menuItemSettings.Click += (sender, e) =>
            {
                var openWnd = true;
                foreach (Window wnd in System.Windows.Application.Current.Windows)
                {
                    if (wnd is wndSettings)
                    {
                        openWnd = false;
                        break;
                    }
                }

                if (openWnd)
                {
                    var wndSettings = new wndSettings();
                    wndSettings.BringIntoView();
                    wndSettings.Focus();
                    wndSettings.Show();
                }
            };

            MenuItem menuItemClose = new MenuItem("Close");
            menuItemClose.Click += MenuItemClose_Click;
            
            MenuItem menuItemTryNow = new MenuItem("Run now..");
            menuItemTryNow.Click += (sender, e) =>
            {
                viewModel.GetLastestEarthBackground();
            };

            MenuItem menuItemStart = new MenuItem("Start");
            menuItemStart.Checked = Settings.GetIsStart();
            menuItemStart.Click += MenuItemSetStart_Click;

            MenuItem menuItemStop = new MenuItem("Stop");
            menuItemStop.Checked = menuItemStart.Checked ? false : true;
            menuItemStop.Click += MenuItemSetStart_Click;
            
            menuItemApplication.MenuItems.Add(menuItemStart);
            menuItemApplication.MenuItems.Add(menuItemStop);

            trayMenu.MenuItems.Add(menuItemApplication);
            trayMenu.MenuItems.Add(menuItemSettings);
            trayMenu.MenuItems.Add(menuItemClose);
            trayMenu.MenuItems.Add("-");//Separator. yeah i know wtf!
            trayMenu.MenuItems.Add(menuItemTryNow);

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;

            viewModel.SetBallon(ref trayIcon);
            if (Settings.GetIsStart())
                viewModel.StartTimer();
        }

        private void MenuItemSetStart_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            if (menuItem != null)
            {
                var isStart = menuItem.Text.ToLower() == "start" ? true : false;
                Settings.SetIsStart(isStart);

                if (isStart)
                    viewModel.ResetTimer();
                else
                    viewModel.StopTimer();

                SetMenuItems(menuItem);
            }
        }
        
        private void MenuItemClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void SetMenuItems(MenuItem currentSelectedItem)
        {
            var parentItems = currentSelectedItem.Parent;
            foreach (MenuItem item in parentItems.MenuItems)
            {
                item.Checked = false;
            }

            currentSelectedItem.Checked = true;
        }
    }
}
