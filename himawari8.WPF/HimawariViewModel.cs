using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Starting..", $"Starting background timer. Ticking every {Settings.GetUpdateTime()} minutes", ToolTipIcon.Info);

            himawariTimer.Start();
        }

        public void StopTimer()
        {
            himawariTimer.Stop();
            himawariUpdater.Dispose();

            if (Settings.GetShowNotification())
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Stopping..", $"Stopping background timer", ToolTipIcon.Info);
        }

        public void ResetTimer()
        {
            StopTimer();
            SetTimer();
            StartTimer();
        }

        private readonly BackgroundWorker himawariUpdater = new BackgroundWorker();

        public HimawariViewModel()
        {
            himawariUpdater.DoWork += HimawariUpdater_DoWork;
            himawariUpdater.RunWorkerCompleted += HimawariUpdater_RunWorkerCompleted;
            SetTimer();
        }
        
        private void HimawariUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        private void HimawariUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            if (Settings.GetShowNotification())
                Ballon.ShowBalloonTip(Settings.GetNotificationTime(), "Updating..", "Updating earth background", ToolTipIcon.Info);

            GetLastestEarthBackground();
        }

        public void GetLastestEarthBackground()
        {
            var directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            var vbsFile = Path.Combine(directoryPath, Settings.VBSFileName);
            
            if (File.Exists(vbsFile))
            {
                var process = new Process();
                process.StartInfo.WorkingDirectory = directoryPath;
                process.StartInfo.FileName = Settings.VBSFileName;
                process.StartInfo.Arguments = "//B //Nologo";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
        }
    }
}
