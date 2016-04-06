using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SC2LiquipediaStatistics.DesktopClient.Common;
using SC2LiquipediaStatistics.DesktopClient.ViewModel;

using Image = System.Windows.Controls.Image;

namespace SC2LiquipediaStatistics.DesktopClient.View
{
    /// <summary>
    /// Interaction logic for PlayerSelection.xaml
    /// </summary>
    public partial class SoloPlayerStatisticsView
    {
        private readonly double playerImageHeight;
        private readonly double playerImageWidth;

        public SoloPlayerStatisticsView()
        {
            InitializeComponent();

            playerImageWidth = Math.Floor(playerImage.Width);
            playerImageHeight = Math.Floor(playerImage.Height);

            var prop = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            prop.AddValueChanged(playerImage, Image_SourceUpdated);
        }

        private void Image_SourceUpdated(object sender, EventArgs eventArgs)
        {
            var image = sender as Image;

            if (image == null)
                return;

            if (!(image.Source is BitmapSource))
                return;

            if (Math.Floor(image.Source.Width) > playerImageWidth)
            {
                var percentage = playerImageWidth / image.Source.Width;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }

            if (Math.Floor(image.Height) > playerImageHeight)
            {
                var percentage = playerImageHeight / image.Height;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }
        }
    }
}