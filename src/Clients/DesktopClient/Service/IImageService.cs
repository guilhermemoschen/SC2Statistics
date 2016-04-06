using System;
using System.Windows.Media;

namespace SC2LiquipediaStatistics.DesktopClient.Service
{
    public interface IImageService
    {
        ImageSource LoadImage(Uri playerImageUri);
    }
}