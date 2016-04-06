using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SC2LiquipediaStatistics.DesktopClient.View
{
    /// <summary>
    /// Interaction logic for PlayerXPlayerStatisticsView.xaml
    /// </summary>
    public partial class PlayerXPlayerStatisticsView
    {
        private readonly double playersImageHeight;
        private readonly double playersImageWidth;

        public PlayerXPlayerStatisticsView()
        {
            InitializeComponent();

            playersImageWidth = Math.Floor(player1Image.Width);
            playersImageHeight = Math.Floor(player1Image.Height);

            var imagePlayer1Property = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            imagePlayer1Property.AddValueChanged(player1Image, Image_SourceUpdated);

            var imagePlayer2Property = DependencyPropertyDescriptor.FromProperty(Image.SourceProperty, typeof(Image));
            imagePlayer2Property.AddValueChanged(player2Image, Image_SourceUpdated);
        }

        private void Image_SourceUpdated(object sender, EventArgs eventArgs)
        {
            var image = sender as Image;

            if (image == null)
                return;

            if (!(image.Source is BitmapSource))
                return;

            if (Math.Floor(image.Source.Width) > playersImageWidth)
            {
                var percentage = playersImageWidth / image.Source.Width;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }

            if (Math.Floor(image.Height) > playersImageHeight)
            {
                var percentage = playersImageHeight / image.Height;
                var newImageSouce = new TransformedBitmap((BitmapSource)image.Source, new ScaleTransform(percentage, percentage));
                image.SetCurrentValue(Image.SourceProperty, newImageSouce);
            }
        }
    }
}
