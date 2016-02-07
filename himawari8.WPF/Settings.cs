
namespace himawari8.WPF
{
    public class Settings
    {
        public const string VBSFileName = "himawari.vbs";
        public const string PLANET_ICON = "planet32.ico";

        public static void SetUpdateTime(int time)
        {
            Properties.Settings.Default.Update = time;

            Properties.Settings.Default.Save();
        }
        public static int GetUpdateTime()
        {
            return Properties.Settings.Default.Update;
        }

        public static void SetIsStart(bool value)
        {
            Properties.Settings.Default.IsStart = value;
            Properties.Settings.Default.Save();
        }
        public static bool GetIsStart()
        {
            var isStart = Properties.Settings.Default.IsStart;

            return isStart;
        }

        public static bool GetShowNotification()
        {
            return Properties.Settings.Default.ShowNotification;
        }
        public static void SetShowNotification(bool value)
        {
            Properties.Settings.Default.ShowNotification = value;
            Properties.Settings.Default.Save();
        }

        public static void SetNotificationTime(int time)
        {
            Properties.Settings.Default.NotificationTime = time;

            Properties.Settings.Default.Save();
        }
        public static int GetNotificationTime()
        {
            return Properties.Settings.Default.NotificationTime;
        }

        public static bool GetStartUp()
        {
            return Properties.Settings.Default.Startup;
        }
        public static void SetStartUp(bool value)
        {
            Properties.Settings.Default.Startup = value;
            Properties.Settings.Default.Save();
        }

        public static void SetWpStyle(string style)
        {
            Properties.Settings.Default.wpStyle = style;

            Properties.Settings.Default.Save();
        }
        public static string GetWpStyle()
        {
            return Properties.Settings.Default.wpStyle;
        }

        public static void SetWpQuality(string quality)
        {
            Properties.Settings.Default.wpQuality = quality;

            Properties.Settings.Default.Save();
        }
        public static string GetWpQuality()
        {
            return Properties.Settings.Default.wpQuality;
        }

        public static bool GetSaveWallpaper()
        {
            return Properties.Settings.Default.SaveWallpaper;
        }
        public static void SetSaveWallpaper(bool value)
        {
            Properties.Settings.Default.SaveWallpaper = value;
            Properties.Settings.Default.Save();
        }
    }
}
