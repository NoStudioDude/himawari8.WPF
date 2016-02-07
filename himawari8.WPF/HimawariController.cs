using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace himawari8.WPF
{
    public class HimawariController
    {
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style
        {
            Tiled = 0,
            Centered = 1,
            Stretched = 2,
            Fill = 3,
            Fit = 4
        }
        public enum Quality
        {
            Basic = 0,
            Good = 1,
            HD = 2,
            Ultra = 3
        }

        private Quality _quality = Quality.Basic;
        private Style _wallpaperStyle = Style.Fit;
        private string LEVEL; //Level can be 4d, 8d, 16d, 20d
        private int NUM_BLOCKS; // Keep this number the same as level
        private string FILE_EXTENSION;
        
        private const int WIDTH = 550;
        private const string HIMAWARI_FOLDER_NAME = "Himawari";
        private const string HIMAWARI_SAVED_FOLDER_NAME = "Saved";

        private static DateTime GetCurrentDateTime()
        {
            var now = DateTime.Now;
            now = now.AddHours(-2);
            now = now.AddMinutes(-30);

            var modMin = now.Minute % 10; 
            now = now.AddMinutes(-(modMin)); // get time rounded to 10 minutes. Himawari does only provide images when minutes are mod of 10

            return now;
        }

        private DateTime now;
        private string _time;
        private string _year;
        private string _month;
        private string _day;
        
        private string _outputPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), HIMAWARI_FOLDER_NAME);
            }
        }

        private string _outputFileName
        {
            get
            {
                return $"latest.{FILE_EXTENSION}";
            }
        }

        private string url => $"http://himawari8-dl.nict.go.jp/himawari8/img/D531106/{LEVEL}/{WIDTH}/{_year}/{_month}/{_day}/{_time}00";
        private Bitmap image;

        public HimawariController()
        {
            now = GetCurrentDateTime();
            _time = now.ToString("HHmm");
            _year = now.ToString("yyyy");
            _month = now.ToString("MM");
            _day = now.ToString("dd");

            var style = Settings.GetWpStyle();
            _wallpaperStyle = (Style)Enum.Parse(typeof(Style), style);

            var quality = Settings.GetWpQuality();
            _quality = (Quality)Enum.Parse(typeof(Quality), quality);
            switch (_quality)
            {
                case Quality.Basic :
                    LEVEL = "4d";
                    NUM_BLOCKS = 4;
                    FILE_EXTENSION = "bmp";
                    break;
                case Quality.Good:
                    LEVEL = "8d";
                    NUM_BLOCKS = 8;
                    FILE_EXTENSION = "jpg";
                    break;
                case Quality.HD:
                    LEVEL = "16d";
                    NUM_BLOCKS = 16;
                    FILE_EXTENSION = "png";
                    break;
                case Quality.Ultra:
                    LEVEL = "20d";
                    NUM_BLOCKS = 20;
                    FILE_EXTENSION = "png";
                    break;
            }

            image = new Bitmap((WIDTH * NUM_BLOCKS), (WIDTH * NUM_BLOCKS));
            
            var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), HIMAWARI_FOLDER_NAME);
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            var outputTimelapse = Path.Combine(output, HIMAWARI_SAVED_FOLDER_NAME);
            if (!Directory.Exists(outputTimelapse))
                Directory.CreateDirectory(outputTimelapse);
        }

        public void BuildWallpaper()
        {
            var graphics = Graphics.FromImage(image);
            
            for (int y = 0; y < NUM_BLOCKS; y++)
            {
                for (int x = 0; x < NUM_BLOCKS; x++)
                {
                    var thisUrl = $"{url}_{x.ToString()}_{y.ToString()}.png";
                    try
                    {
                        var request = WebRequest.Create(thisUrl);
                        var response = request.GetResponse();
                        if (response != null)
                        {
                            var imageBlock = Image.FromStream(response.GetResponseStream());
                            graphics.DrawImage(imageBlock, (x* WIDTH), (y* WIDTH), WIDTH, WIDTH);
                            
                            imageBlock.Dispose();
                            response.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.Message);
                    }
                }
            }

            var latest = Path.Combine(_outputPath, _outputFileName);
            image.Save(latest, GetImageFormat());
            image.Dispose();

            if (Settings.GetSaveWallpaper())
            {
                var folderPath = Path.Combine(_outputPath, HIMAWARI_SAVED_FOLDER_NAME);
                var filename = $"{_year}{_month}{_day}_{_time}.{FILE_EXTENSION}";
                var destination = Path.Combine(folderPath, filename);

                File.Copy(latest, destination);
            }

            SetWallpaper();
        }

        private ImageFormat GetImageFormat()
        {
            if (FILE_EXTENSION == "bmp")
                return ImageFormat.Bmp;

            if (FILE_EXTENSION == "jpg")
                return ImageFormat.Jpeg;

            if (FILE_EXTENSION == "png")
                return ImageFormat.Png;

            return ImageFormat.Jpeg;
        }

        private void SetWallpaper()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

            switch (_wallpaperStyle)
            {
                case Style.Centered:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Fill:
                    key.SetValue(@"WallpaperStyle", "10");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Fit:
                    key.SetValue(@"WallpaperStyle", "6");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Stretched:
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Tiled:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "1");
                    break;
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                Path.Combine(_outputPath, _outputFileName),
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
