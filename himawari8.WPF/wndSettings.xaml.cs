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

            txtUpdateTime.Text = Settings.GetUpdateTime().ToString();
            ckShowNotification.IsChecked = Settings.GetShowNotification();
            txtNotificationTime.Text = Settings.GetNotificationTime().ToString();
                        
            ckStartup.IsChecked = Settings.GetStartUp();
        }

        private void wndSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var updateTime = 0;

            if (int.TryParse(txtUpdateTime.Text, out updateTime))
            {
                Settings.SetUpdateTime(updateTime);
            }

            Settings.SetShowNotification((bool)ckShowNotification.IsChecked);
            Settings.SetStartUp((bool)ckStartup.IsChecked);
            CreateShortcut();
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
