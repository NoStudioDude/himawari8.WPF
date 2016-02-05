using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Threading;

namespace himawari8.WPF
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    
    public class HimawariViewModel : ViewModelBase
    {
        private NotifyIcon _ballon;
        public NotifyIcon Ballon
        {
            get { return _ballon; }
            set {
                _ballon = value;
                OnPropertyChanged();
            }
        }

        public void SetBallon(ref NotifyIcon ballon)
        {
            Ballon = ballon;
        }

        private DispatcherTimer himawariTimer;
        private void SetTimer()
        {
            himawariTimer = new DispatcherTimer();
            himawariTimer.Tick += new EventHandler(himawariTimer_Tick);
            himawariTimer.Interval = new TimeSpan(0, Settings.GetUpdateTime(), 0);
        }
        private void himawariTimer_Tick(object sender, EventArgs e)
        {
            if (!himawariUpdater.IsBusy)
                himawariUpdater.RunWorkerAsync();
        }

        public void StartTimer()
        {
            if(Settings.GetShowNotification())
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Starting..", $"Background worker initiated. Updating wallpaper every {Settings.GetUpdateTime()} minutes", ToolTipIcon.Info);

            himawariTimer.Start();
        }

        public void StopTimer(bool isReseting = false)
        {
            himawariTimer.Stop();
            himawariUpdater.Dispose();

            if (Settings.GetShowNotification() && !isReseting)
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Stopping..", "Stopping background worker", ToolTipIcon.Info);
        }

        public void ResetTimer()
        {
            if(himawariTimer.IsEnabled)
                StopTimer(true);

            SetTimer();
            StartTimer();
        }

        private readonly BackgroundWorker himawariUpdater = new BackgroundWorker();

        private HimawariController himawariController;

        public HimawariViewModel()
        {
            himawariUpdater.DoWork += HimawariUpdater_DoWork;
            himawariUpdater.RunWorkerCompleted += HimawariUpdater_RunWorkerCompleted;
            SetTimer();
        }
        
        private void HimawariUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Settings.GetShowNotification())
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Wallpaper..", $"Setting desktop wallpaper", ToolTipIcon.Info);
        }

        private void HimawariUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.GetShowNotification())
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Updating..", "Download latest earth images..", ToolTipIcon.Info);

            GetLastestEarthBackground();
        }

        public void GetLastestEarthBackground()
        {
            if (CanPingGoogle())
            {
                himawariController = new HimawariController(Settings.GetTimeLapse());
                himawariController.BuildWallpaper();
            }
            else
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Internet is unavailable", "Please make sure you have an active internet connection.", ToolTipIcon.Warning);
        }

        private static bool CanPingGoogle()
        {
            const int timeout = 1000;
            const string host = "google.com";

            var ping = new Ping();
            var buffer = new byte[32];
            var pingOptions = new PingOptions();

            try
            {
                var reply = ping.Send(host, timeout, buffer, pingOptions);
                return (reply != null && reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
