using IWshRuntimeLibrary;
using System;
using System.IO;
using System.Windows;

namespace himawari8.WPF
{
    public partial class wndSettings : Window
    {
        public wndSettings()
        {
            InitializeComponent();

            var updater = Settings.GetUpdateTime();
            cbUpdater.Items.Add(10);
            cbUpdater.Items.Add(15);
            cbUpdater.Items.Add(30);
            cbUpdater.Items.Add(45);
            cbUpdater.Items.Add(60);

            foreach (int u in cbUpdater.Items)
            {
                if (u == updater)
                {
                    cbUpdater.SelectedItem = u;
                    break;
                }
            }

            ckShowNotification.IsChecked = Settings.GetShowNotification();
            txtNotificationTime.Text = Settings.GetNotificationTime().ToString();
                        
            ckStartup.IsChecked = Settings.GetStartUp();

            var quality = Settings.GetWpQuality();
            cbQuality.Items.Add("Basic");//4d
            cbQuality.Items.Add("Good");//8d
            cbQuality.Items.Add("HD");//16d
            cbQuality.Items.Add("Ultra");//20d

            foreach (var q in cbQuality.Items)
            {
                if (q.ToString() == quality)
                {
                    cbQuality.SelectedItem = q;
                    break;
                }
            }

            var style = Settings.GetWpStyle();
            cbStyle.Items.Add("Tiled");
            cbStyle.Items.Add("Centered");
            cbStyle.Items.Add("Stretched");
            cbStyle.Items.Add("Fill");
            cbStyle.Items.Add("Fit");
            foreach (var s in cbStyle.Items)
            {
                if (s.ToString() == style)
                {
                    cbStyle.SelectedItem = s;
                    break;
                }
            }

        }

        private void wndSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var updateTime = 0;

            if (int.TryParse(cbUpdater.SelectedItem.ToString(), out updateTime))
            {
                Settings.SetUpdateTime(updateTime);
            }

            if (int.TryParse(txtNotificationTime.Text, out updateTime))
            {
                Settings.SetNotificationTime(updateTime);
            }

            Settings.SetShowNotification((bool)ckShowNotification.IsChecked);
            Settings.SetStartUp((bool)ckStartup.IsChecked);
            CreateShortcut();

            Settings.SetWpQuality(cbQuality.SelectedItem.ToString());
            Settings.SetWpStyle(cbStyle.SelectedItem.ToString());
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            txtNotificationTime.Visibility = (bool)ckShowNotification.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CreateShortcut()
        {
            string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "himawari8.lnk");

            if (!System.IO.File.Exists(shortcutLocation) && Settings.GetStartUp())
            {
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                var targetFileLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
                var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons");
                var iconPath = Path.Combine(directoryPath, Settings.PLANET_ICON);

                shortcut.Description = "Earth from Himawari8";
                shortcut.IconLocation = iconPath;
                shortcut.TargetPath = targetFileLocation;
                shortcut.Save();
            }
            else if (System.IO.File.Exists(shortcutLocation) && !Settings.GetStartUp())
            {
                System.IO.File.Delete(shortcutLocation);
            }
        }
    }
}
