using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SC2Statistics.Utilities.Web;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public class ImageService : IImageService
    {
        public IDownloader Downloader { get; private set; }

        public ImageService(IDownloader downloader)
        {
            Downloader = downloader;
        }

        public ImageSource LoadImage(Uri playerImageUri)
        {
            var imageData = Downloader.GetContentAsBytes(playerImageUri);
            var bitmapImage = new BitmapImage();

            using (var stream = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return bitmapImage;
        }
    }
}
